// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

///http://harismoonamkunnu.blogspot.co.uk/2013/06/opencv-find-biggest-contour-using-c.html


int iLowH = 0;
int iHighH = 179;

int iLowS = 0; 
int iHighS = 78;

int iLowV = 0;
int iHighV = 255;

int iMinWidth = 300;
int iMinHeight = 300;


EXTERN_DLL_EXPORT 
void __stdcall ReadSettings(int &highS, int &minWidth, int &minHeight)
{
	highS = iHighS;
	minWidth = iMinWidth;
	minHeight = iMinHeight;
}

EXTERN_DLL_EXPORT 
void __stdcall SaveSettings(int highS, int minWidth, int minHeight)
{
	if(highS >=0)
		iHighS = highS;

	if(minWidth >=0)
		iMinWidth = minWidth;

	if(minHeight >=0)
		iMinHeight = minHeight;
}

EXTERN_DLL_EXPORT 
void __stdcall ProcessImage(IplImage image, IplImage thresholded, IplImage hsv, Rect& bounding_rect)
{
	Mat imgOriginal0(&image, false);
	Mat imgThresholded(&thresholded, false);

	Mat imgHSV(&hsv, false);

	cvtColor(imgOriginal0, imgHSV, COLOR_BGR2HSV); //Convert the captured frame from BGR to HSV

	inRange(imgHSV, Scalar(iLowH, iLowS, iLowV), Scalar(iHighH, iHighS, iHighV), imgThresholded); //Threshold the image
      
	//morphological opening (removes small objects from the foreground)
	erode(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) );
	dilate(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) ); 

	//morphological closing (removes small holes from the foreground)
	dilate(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) ); 
	erode(imgThresholded, imgThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)) );
		
	vector<vector<Point> > contours;
	vector<Vec4i> hierarchy;
	cv::findContours( imgThresholded, contours, hierarchy,CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

	 int largest_area = 0;
	 int largest_contour_index=0;

	for( int i = 0; i< contours.size(); i++ ) // iterate through each contour. 
	{
		double current = contourArea( contours[i],false);  //  Find the area of contour
		if(current > largest_area)
		{
			largest_area = current;
			largest_contour_index=i;                //Store the index of largest contour
			bounding_rect=boundingRect(contours[i]); // Find the bounding rectangle for biggest contour
		}
	}

	if(bounding_rect.width > iMinWidth && bounding_rect.height > iMinHeight)
	{
		Scalar color(244);//( 255,255,255);
		drawContours(imgOriginal0, contours,largest_contour_index, color, CV_FILLED, 8, hierarchy ); // Draw the largest contour using previously stored index.
		rectangle(imgOriginal0, bounding_rect, color, 2);  
			
		//Mat resultArea(imgOriginal0, bounding_rect);
		//result = resultArea.clone();
		//imshow("Result", result); 
		//result.copyTo(imgOriginal0);
	}
}