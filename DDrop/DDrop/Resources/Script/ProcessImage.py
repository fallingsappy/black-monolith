import sys
import cv2
import numpy as np
from os.path import basename, splitext

def main():
    input_path = sys.argv[1]
    print(input_path)
    output_path = sys.argv[2]
    print(output_path)
    image = load_image(input_path)
    image_name = load_image_name(input_path)
    diameters = process_image(image, output_path)
    print(diameters)
    return diameters
 

def load_image(input_path):
    image = cv2.imread(input_path)
    return image


def load_image_name(input_path):
    image_name, _ = splitext(basename(input_path))
    return image_name


def process_image(image, output_path):
    try:       
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        blur = cv2.medianBlur(gray, 9)

        edged = cv2.Canny(blur, 50, 5)
        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (100, 250))
        closed = cv2.morphologyEx(edged, cv2.MORPH_CLOSE, kernel)
        contours, _ = cv2.findContours(closed, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        contour_list = []
        for contour in contours:
            approx = cv2.approxPolyDP(contour,0.01*cv2.arcLength(contour,True),True)
            area = cv2.contourArea(contour)
            if ((len(approx) > 8) & (50000 > area > 10000)):
                contour_list.append(contour)

        cv2.drawContours(image, contour_list, -1, (255,0,0), 2)

        cnt = contour_list[0]
        left = tuple(cnt[cnt[:,:,0].argmin()][0])
        right = tuple(cnt[cnt[:,:,0].argmax()][0])
        top = tuple(cnt[cnt[:,:,1].argmin()][0])
        bottom = tuple(cnt[cnt[:,:,1].argmax()][0])

        right_2 = tuple([right[0], left[1]])
        top_2 = tuple([bottom[0], top[1]])

        cv2.circle(image, left, 6, (0, 0, 255), -1)
        cv2.circle(image, right_2, 6, (0, 0, 255), -1)
        cv2.circle(image, top_2, 6, (0, 255, 0), -1)
        cv2.circle(image, bottom, 6, (0, 255, 0), -1)

        distance_1 = np.sqrt((right_2[0] - left[0])**2 + (right_2[1] - left[1])**2)
        distance_2 = np.sqrt((top_2[0] - bottom[0])**2 + (top_2[1] - bottom[1])**2)
        diameters = [distance_1, distance_2]

        font = cv2.FONT_HERSHEY_SIMPLEX
        cv2.line(image, left, right_2, (0,0,255), 2)
        cv2.line(image, top_2, bottom, (0,255,0), 2)
        cv2.putText(image,'Distance between right and left: ' + str(distance_1) +
                    '. Distance between top and bottom: ' +
                    str(distance_2) ,(100,300), font, 2, (0,0,0),2, cv2.LINE_AA)
        return diameters
    except cv2.error:
        print ('file is not an image')

    return 0

main()


