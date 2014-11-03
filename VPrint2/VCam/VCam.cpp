// This is the main DLL file.

#include "stdafx.h"

#include <iostream>
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"

using namespace cv;
using namespace std;

////http://harismoonamkunnu.blogspot.co.uk/2013/06/opencv-find-biggest-contour-using-c.html
///http://opencv-srf.blogspot.co.uk/2010/09/object-detection-using-color-seperation.html
///
 int main( int argc, char** argv )
 {
    VideoCapture cap(0); //capture the video from web cam

    if ( !cap.isOpened() )  // if not success, exit program
    {
         cout << "Cannot open the web cam" << endl;
         return -1;
    }

    namedWindow("Control", CV_WINDOW_AUTOSIZE); //create a window called "Control"

	int iLowH = 0;
	int iHighH = 179;

	int iLowS = 0; 
	int iHighS = 78;

	int iLowV = 0;
	int iHighV = 255;

	//Create trackbars in "Control" window
	cvCreateTrackbar("LowH", "Control", &iLowH, 179); //Hue (0 - 179)
	cvCreateTrackbar("HighH", "Control", &iHighH, 179);

	cvCreateTrackbar("LowS", "Control", &iLowS, 255); //Saturation (0 - 255)
	cvCreateTrackbar("HighS", "Control", &iHighS, 255);

	cvCreateTrackbar("LowV", "Control", &iLowV, 255);//Value (0 - 255)
	cvCreateTrackbar("HighV", "Control", &iHighV, 255);

	int iMinWidth = 300;
	int iMinHeight = 300;

	cvCreateTrackbar("Min Width", "Control", &iMinWidth, 2000);//Value (0 - 2000)
	cvCreateTrackbar("Min Height", "Control", &iMinHeight, 2000);

	namedWindow("Result", CV_WINDOW_AUTOSIZE);

    while (true)
    {
        Mat imgOriginal;

        bool bSuccess = cap.read(imgOriginal); // read a new frame from video

         if (!bSuccess) //if not success, break loop
        {
             cout << "Cannot read a frame from video stream" << endl;
             break;
        }

		Mat imgHSV;

		cvtColor(imgOriginal, imgHSV, COLOR_BGR2HSV); //Convert the captured frame from BGR to HSV
	
		Mat imgThresholded;

		inRange(imgHSV, Scalar(iLowH, iLowS, iLowV), Scalar(iHighH, iHighS, iHighV), imgThresholded); //Threshold the image
      
		//morphological opening (removes small objects from the foreground)
		erode(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) );
		dilate(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) ); 

		//morphological closing (removes small holes from the foreground)
		dilate(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) ); 
		erode(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) );
		
		imshow("Thresholded Image", imgThresholded); //show the thresholded image

		vector<vector<Point> > contours;
		cv::findContours(imgThresholded, contours, cv::RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);
		for (size_t i=0; i<contours.size(); ++i)
		{
			// do something with the current contour
			// for instance, find its bounding rectangle
			Rect r = cv::boundingRect(contours[i]);
			if(r.width > iMinWidth && r.height > iMinHeight)
			{
				Mat resultArea(imgOriginal, r);
		   		Mat result = resultArea.clone();
			
				rectangle(imgOriginal, r, Scalar(244), 2);
				imshow("Result", result); 
			}
		}

		
		imshow("Original", imgOriginal); //show the original image

        if (waitKey(30) == 27) //wait for 'esc' key press for 30ms. If 'esc' key is pressed, break loop
       {
            cout << "esc key is pressed by user" << endl;
            break; 
       }
    }

   return 0;
}
