#server.py

from flask import Flask, jsonify, request
import subprocess
import sys
import os
import locale
import unicodedata

app = Flask(__name__)

# Set UTF-8 locale globally for subprocesses if needed
try:
    locale.setlocale(locale.LC_ALL, 'C.UTF-8')
except locale.Error:
    pass

# Helper function to clean text for subprocess compatibility
def safe_text(text):
    return unicodedata.normalize('NFKD', text).encode('ASCII', 'ignore').decode('ASCII')

# Global variable to hold the current lesson info
current_lesson_info = {
    "lesson_id": None,
    "lesson_name": "Unknown"
}

@app.route('/set_current_lesson', methods=['POST'])
def set_current_lesson():
    """
    Set the current lesson info from frontend JSON
    """
    global current_lesson_info
    data = request.get_json()
    lesson_id = data.get("lesson_id")
    lesson_name = data.get("lesson_name", "Unknown")

    if not lesson_id:
        return jsonify({"error": "Lesson ID is required"}), 400

    current_lesson_info["lesson_id"] = lesson_id
    current_lesson_info["lesson_name"] = safe_text(lesson_name)

    return jsonify({
        "message": f"Current lesson set to: {lesson_id} ({lesson_name})",
        "status": "success"
    })

@app.route('/get_current_lesson', methods=['GET'])
def get_current_lesson_endpoint():
    global current_lesson_info
    return jsonify({
        "lesson_id": current_lesson_info.get("lesson_id"),
        "lesson_name": current_lesson_info.get("lesson_name", "Unknown"),
        "institution_id": current_lesson_info.get("institution_id")
    })

@app.route('/take_attendance', methods=['POST'])
def take_attendance():
    try:
        data = request.get_json()
        lesson_id = data.get('lesson_id')
        lesson_name = data.get('lesson_name', 'Unknown')
        institution_id = data.get('institution_id')

        if not lesson_id or not institution_id:
            return jsonify({"error": "Lesson ID and Institution ID are required"}), 400

        global current_lesson_info
        current_lesson_info["lesson_id"] = lesson_id
        current_lesson_info["lesson_name"] = safe_text(lesson_name)
        current_lesson_info["institution_id"] = institution_id  

        # Avoid relaunch
        already_running = any('face_recognition_attendance.py' in line for line in os.popen('tasklist' if os.name == 'nt' else 'ps aux'))

        if not already_running:
            subprocess.Popen(
                [sys.executable, "face_recognition_attendance.py"],
                shell=(os.name == 'nt'),
                env={**os.environ, 'PYTHONIOENCODING': 'utf-8'}
            )
            status = "running"
            message = "Attendance capture started"
        else:
            status = "already_running"
            message = "Attendance capture already running"

        return jsonify({
            "message": message,
            "lesson_id": lesson_id,
            "lesson_name": lesson_name,
            "institution_id": institution_id,
            "status": status
        })
    except Exception as e:
        return jsonify({"error": str(e)}), 500


@app.route('/encode_faces', methods=['GET'])
def encode_faces_endpoint():
    try:
        python_executable = sys.executable
        env = os.environ.copy()
        env['PYTHONIOENCODING'] = 'utf-8'
        process = subprocess.run([python_executable, "encode_faces.py"], 
                                 capture_output=True, text=True, env=env)

        if process.returncode != 0:
            return jsonify({"error": process.stderr}), 500

        return jsonify({
            "message": "Face encoding completed successfully", 
            "output": process.stdout
        })
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.errorhandler(500)
def internal_error(error):
    return jsonify({"error": "Server Error"}), 500

@app.errorhandler(404)
def not_found(error):
    return jsonify({"error": "Endpoint not found"}), 404

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=False, threaded=True)