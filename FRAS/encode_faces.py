# encode_faces.py
import face_recognition
import os
import pickle
import cv2

def encode_faces(image_folder='faces'):
    """
    Encode all faces found in the specified directory
    """
    known_encodings = []
    known_names = []
    
    # Ensure the folder exists
    if not os.path.exists(image_folder):
        print(f"Error: Folder '{image_folder}' does not exist")
        return known_encodings, known_names
    
    face_count = 0
    
    for filename in os.listdir(image_folder):
        if filename.lower().endswith((".jpg", ".jpeg", ".png")):
            image_path = os.path.join(image_folder, filename)
            print(f"Processing {filename}...")
            
            try:
                # Load the image
                image = face_recognition.load_image_file(image_path)
                
                # Find face encodings
                encodings = face_recognition.face_encodings(image)
                
                if encodings:
                    # Use the first face found
                    encoding = encodings[0]
                    known_encodings.append(encoding)
                    known_names.append(os.path.splitext(filename)[0])
                    face_count += 1
                    print(f"  Successfully encoded {filename}")
                else:
                    print(f"  No face found in {filename}")
            except Exception as e:
                print(f"  Error processing {filename}: {e}")
    
    print(f"Encoded {face_count} faces out of {len(os.listdir(image_folder))} images")
    return known_encodings, known_names

if __name__ == "__main__":
    print("Starting face encoding process...")
    known_encodings, known_names = encode_faces()
    
    if known_encodings:
        with open('encodings.pkl', 'wb') as f:
            pickle.dump((known_encodings, known_names), f)
        print(f"Successfully saved encodings for {len(known_names)} faces to encodings.pkl")
    else:
        print("No faces were encoded. Please check your images.")