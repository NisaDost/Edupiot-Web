#face_recognition_attendance.py
# -*- coding: utf-8 -*-
import cv2
import face_recognition
import pickle
import pyodbc
import numpy as np
from datetime import datetime, timedelta
import requests
import os
import uuid

# Database connection
connection_string = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=edupilot;"
    "Trusted_Connection=yes;"
)

def get_current_lesson():
    try:
        response = requests.get('http://localhost:5000/get_current_lesson')
        data = response.json()
        return data.get('lesson_id'), data.get('lesson_name', 'Unknown'), data.get('institution_id')
    except Exception as e:
        print(f"Error getting current lesson: {e}")
        return None, "Unknown"

def get_student_id_by_mugshot(cursor, mugshot):
    try:
        student_name = os.path.splitext(mugshot)[0]
        cursor.execute("SELECT Id FROM Students WHERE Mugshot = ?", (f"{student_name}.png",))
        row = cursor.fetchone()
        if row:
            return row[0]
        else:
            print(f"No student found with mugshot: {mugshot}")
            return None
    except Exception as e:
        print(f"Database error: {e}")
        return None

def mark_attendance(cursor, conn, student_id, lesson_id, emotion, institution_id):
    try:
        if not student_id or not lesson_id or not institution_id:
            print("Missing parameters!")
            return False

        now = datetime.now()
        date = now.strftime('%Y-%m-%d %H:%M:%S')
        is_present = True
        attendance_id = str(uuid.uuid4())
        institution_id = str(institution_id)  

        cursor.execute(
            "INSERT INTO Attendances (Id, StudentId, LessonId, Date, IsPresent, Emotion, InstitutionId) VALUES (?, ?, ?, ?, ?, ?, ?)",
            (attendance_id, student_id, lesson_id, date, is_present, emotion, institution_id)
        )

        conn.commit()
        print(f"Marked attendance for student {student_id} in lesson {lesson_id} at {date}. Emotion: {emotion}")
        return True
    except Exception as e:
        print(f"Error marking attendance: {e}")
        conn.rollback()
        return False

def main():
    try:
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()
        print("Connected to database successfully")
    except Exception as e:
        print(f"Database connection error: {e}")
        return

    try:
        with open('encodings.pkl', 'rb') as f:
            known_encodings, known_names = pickle.load(f)
        print(f"Loaded {len(known_names)} face encodings")
    except Exception as e:
        print(f"Error loading face encodings: {e}")
        return

    try:
        emotion_model = cv2.dnn.readNetFromONNX("emotion-ferplus-8.onnx")
        emotion_labels = ["Neutral", "Happy", "Sad", "Surprise", "Angry", "Disgust", "Fear", "Contempt"]
        print("Loaded emotion detection model")
    except Exception as e:
        print(f"Error loading emotion model: {e}")
        return

    video_capture = cv2.VideoCapture(0)
    if not video_capture.isOpened():
        print("Failed to open webcam")
        return

    last_mark_time = {}
    lesson_id, lesson_name, institution_id = get_current_lesson()  # Baþlangýçta 1 kere alýnýr

    print("Starting face recognition and emotion detection...")

    while True:
        ret, frame = video_capture.read()
        if not ret:
            print("Failed to capture frame")
            break

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        face_locations = face_recognition.face_locations(rgb_frame)
        face_encodings = face_recognition.face_encodings(rgb_frame, face_locations)

        for face_encoding, face_location in zip(face_encodings, face_locations):
            matches = face_recognition.compare_faces(known_encodings, face_encoding)
            name = "Unknown"
            emotion = "Unknown"

            top, right, bottom, left = face_location
            face_roi = frame[max(0, top):min(bottom, frame.shape[0]), max(0, left):min(right, frame.shape[1])]

            if face_roi.size > 0:
                try:
                    gray_face = cv2.cvtColor(face_roi, cv2.COLOR_BGR2GRAY)
                    resized_face = cv2.resize(gray_face, (64, 64))
                    blob = cv2.dnn.blobFromImage(resized_face, 1.0, (64, 64), 0, swapRB=False)
                    emotion_model.setInput(blob)
                    emotion_preds = emotion_model.forward()
                    emotion_index = np.argmax(emotion_preds)
                    emotion = emotion_labels[emotion_index]
                except Exception as e:
                    print(f"Error in emotion detection: {e}")
                    emotion = "Neutral"

            if any(matches):
                face_distances = face_recognition.face_distance(known_encodings, face_encoding)
                best_match_index = np.argmin(face_distances)

                if matches[best_match_index]:
                    name = known_names[best_match_index]
                    now = datetime.now()
                    if lesson_id and (name not in last_mark_time or (now - last_mark_time[name]) > timedelta(seconds=30)):
                        student_id = get_student_id_by_mugshot(cursor, name)
                        if student_id:
                            if mark_attendance(cursor, conn, student_id, lesson_id, emotion, institution_id):
                                last_mark_time[name] = now
                        else:
                            print(f"Could not find student ID for {name}")

            cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)
            cv2.putText(frame, f"{name}", (left, top - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)
            cv2.putText(frame, f"Emotion: {emotion}", (left, bottom + 20), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 0, 0), 2)

        now_str = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
        text = f"{lesson_name} | {now_str}"
        text_size, _ = cv2.getTextSize(text, cv2.FONT_HERSHEY_SIMPLEX, 0.6, 2)
        text_width = text_size[0]
        cv2.putText(frame, text,
                    (frame.shape[1] - text_width - 10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6,
                    (0, 0, 0), 4, cv2.LINE_AA)
        cv2.putText(frame, text,
                    (frame.shape[1] - text_width - 10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6,
                    (255, 255, 255), 1, cv2.LINE_AA)

        cv2.imshow('Face Recognition and Emotion Detection', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

        if cv2.getWindowProperty('Face Recognition and Emotion Detection', cv2.WND_PROP_VISIBLE) < 1:
            break

    video_capture.release()
    cv2.destroyAllWindows()
    conn.close()
    print("Resources released, application closed")

if __name__ == "__main__":
    main()
