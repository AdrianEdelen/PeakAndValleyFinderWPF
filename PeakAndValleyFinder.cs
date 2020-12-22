using System;
using System.Collections.Generic;
using System.Text;

namespace AutoneticsPeakAndValley
{
    class PeakAndValleyFinder
    {
        private Point[] AllPoints { get; set; }
        private List<Point> Peaks { get; set; }
        private List<Point> Valleys { get; set; }
        private int CurrentPeroidIndex { get; set; }


        public PeakAndValleyFinder(Point[] allPoints)
        {
            AllPoints = allPoints;
        }

        //determine Peaks and Determine Valleys checks that if the method has been run already
        //and returns the already determined value if it is not null
        //this allows you to call the method again if needed without doing all the calculations again
        public IEnumerable<Point> DeterminePeaks()
        {
            if (Peaks == null)
            {
                FindPeaksAndValleys();
            }

            return Peaks;
        }

        public IEnumerable<Point> DetermineValleys()
        {
            if (Valleys == null)
            {
                FindPeaksAndValleys();
            }

            return Valleys;
        }


        private void FindPeaksAndValleys()
        {
            // initialize peaks and valleys
            CurrentPeroidIndex = 0;
            Peaks = new List<Point>();
            Valleys = new List<Point>();

            //isFindingPeak determines if we are starting with a trend towards a peak or valley, 
            //this sets what the first thing we are looking for is peak/valley
            bool isFindingPeak = AllPoints[0].Y < AllPoints[1].Y;
            //the if/else here sets the first datapoint as a peak or valley based on the trend of the first two datapoints
            if (isFindingPeak)
            {
                Valleys.Add(AllPoints[0]);
            }
            else
            {
                Peaks.Add(AllPoints[0]);
            }
            //we loop over the datapoints, fliiping between finding a peak and a valley as we find each
            while (CurrentPeroidIndex < AllPoints.Length - 1)
            {
                if (isFindingPeak)
                {

                    Peaks.Add(FindNextPeak());
                    isFindingPeak = false;
                }
                else
                {
                    Valleys.Add(FindNextValley());
                    isFindingPeak = true;
                }
            }
        }


        private Point FindNextPeak()
        {
            //currentMax is intermediate, this changes as we iterate until we have found our next peak
            //to the find the peak, we iterate over the data, setting the max value, until we find
            //that the data starts to go the other way, at which point we know that we are moving towards
            //the next valley
            Point currentMax = AllPoints[CurrentPeroidIndex];
            for (int i = CurrentPeroidIndex; i < AllPoints.Length; i++)
            {
                CurrentPeroidIndex = i;
                if (currentMax.Y > AllPoints[i].Y)
                {
                    break;
                }
                else
                {
                    currentMax = AllPoints[i];
                }
            }
            return currentMax;
        }
        //this method acts the same as FindNextPeak
        private Point FindNextValley()
        {
            Point currentMin = AllPoints[CurrentPeroidIndex];
            for (int i = CurrentPeroidIndex; i < AllPoints.Length; i++)
            {
                CurrentPeroidIndex = i;
                if (currentMin.Y < AllPoints[i].Y)
                {
                    break;
                }
                else
                {
                    currentMin = AllPoints[i];
                }
            }
            return currentMin;
        }
    }
}

