import sys
import cv2
import numpy as np
from os.path import basename, splitext

def main():
    input_path = sys.argv[1]
    ksize = int(sys.argv[2])
    treshold1 = int(sys.argv[3])
    treshold2 = int(sys.argv[4])
    size1 = int(sys.argv[5])
    size2 = int(sys.argv[6])
    image = load_image(input_path)
    contours = process_image(image, ksize, treshold1, treshold2, size1, size2)
    print(contours)
    return contours

def process_image(image, ksize, treshold1, treshold2, size1, size2):    
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    blur = cv2.medianBlur(gray, ksize)

    edged = cv2.Canny(blur, treshold1, treshold2)
    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (size1, size2))
    closed = cv2.morphologyEx(edged, cv2.MORPH_CLOSE, kernel)
    contours, _ = cv2.findContours(closed, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    contour_list = []
    for contour in contours:
        approx = cv2.approxPolyDP(contour,0.01*cv2.arcLength(contour,True),True)
        area = cv2.contourArea(contour)
        if ((len(approx) > 8) & (50000 > area > 10000)):
            contour_list.append(contour)

    contours = contour_list[0]

    return contours

def load_image(input_path):
    image = cv2.imread(input_path)
    return image

main()







