import cv2
import numpy as np
import time
import requests


# Define the lower and upper boundaries of the purple color in HSV color space
lower_purple = np.array([130, 50, 50])
upper_purple = np.array([170, 255, 255])

# Create a VideoCapture object to read from the webcam
cap = cv2.VideoCapture(0)

# Store previous center coordinates and their timestamps
previous_centers = []
previous_timestamps = []

while True:
    # Read the current frame from the video feed
    ret, frame = cap.read()
    
    if not ret:
        break
    
    # Convert the frame from BGR to HSV color space
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    
    # Threshold the frame to get only the purple color
    mask = cv2.inRange(hsv, lower_purple, upper_purple)
    
    # Find contours of the purple color in the mask
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    if len(contours) > 0:
        # Find the contour with the largest area
        largest_contour = max(contours, key=cv2.contourArea)
        
        # Calculate the center of the contour
        M = cv2.moments(largest_contour)
        
        if M["m00"] != 0:
            center_x = int(M["m10"] / M["m00"])
            center_y = int(M["m01"] / M["m00"])
            
            # Scale the position of the center coordinates from 0 to 100
            scaled_center_x = int((center_x / frame.shape[1]) * 100)
            scaled_center_y = int((center_y / frame.shape[0]) * 562.5)
            
            # Draw a bounding box around the contour
            x, y, w, h = cv2.boundingRect(largest_contour)
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
            
            # Display the scaled center coordinates on the frame
            cv2.putText(frame, f"({scaled_center_x}, {scaled_center_y})", (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
            
            # Add current center coordinates and timestamp to the list
            previous_centers.append((center_x, center_y))
            previous_timestamps.append(time.time())
            
            # Draw a disappearing trace of the center
            for i in range(len(previous_centers) - 1, -1, -1):
                if time.time() - previous_timestamps[i] <= 3:
                    cv2.line(frame, previous_centers[i-1], previous_centers[i], (0, 0, 255), 2)
                else:
                    del previous_centers[i]
                    del previous_timestamps[i]
                    
            # Output the x and y coordinates of the bounding box to the terminal
            print(f"Bounding Box Coordinates: ({scaled_center_x}, {scaled_center_y})")

            postdata = {"x": scaled_center_x, "y": scaled_center_y }
            r = requests.post("http://165.227.237.10/upload_position", json=postdata)

            print(postdata)
            print(r.status_code)
    
    # Generate the heatmap of the ball
    heatmap = cv2.applyColorMap(mask, cv2.COLORMAP_BONE)
    
    # Resize the heatmap to match the frame size
    heatmap = cv2.resize(heatmap, (frame.shape[1], frame.shape[0]))
    
    # Create a side-by-side view of the frame and the heatmap
    side_by_side = np.hstack((frame, heatmap))
    
    # Display the side-by-side view
    cv2.imshow("Tracking", side_by_side)
    
    # Exit the loop if 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release the VideoCapture object and close all windows
cap.release()
cv2.destroyAllWindows()
