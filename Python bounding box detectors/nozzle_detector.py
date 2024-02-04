import cv2
import mediapipe as mp
from mediapipe.python.solutions import drawing_utils as mp_drawing

# Initialize MediaPipe hands module
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(static_image_mode=False, max_num_hands=2, min_detection_confidence=0.5, min_tracking_confidence=0.5)

# Initialize MediaPipe pose module
mp_pose = mp.solutions.pose
pose = mp_pose.Pose(static_image_mode=False, min_detection_confidence=0.5, min_tracking_confidence=0.5)

# Function to process each frame of the video feed
def process_frame(frame):
    # Convert the frame to RGB color space
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    # Process the frame with MediaPipe hands
    results_hands = hands.process(frame_rgb)

    # Process the frame with MediaPipe pose
    results_pose = pose.process(frame_rgb)

    # Draw hand landmarks and connections
    if results_hands.multi_hand_landmarks:
        for hand_landmarks in results_hands.multi_hand_landmarks:
            mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

    # Draw pose landmarks and connections
    if results_pose.pose_landmarks:
        mp_drawing.draw_landmarks(frame, results_pose.pose_landmarks, mp_pose.POSE_CONNECTIONS)

    # Display the frame with hand and pose landmarks
    cv2.imshow("Hand and Pose Tracking", frame)

    # Wait for key press to exit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        cv2.destroyAllWindows()

# Open the video capture
cap = cv2.VideoCapture(0)

while True:
    # Read a frame from the video capture
    ret, frame = cap.read()

    if ret:
        # Process the frame
        process_frame(frame)
    else:
        break

# Release the video capture
cap.release()
cv2.destroyAllWindows()
