using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Windows;

using System.Windows.Data;

using Microsoft.Win32;


namespace AutoneticsPeakAndValley
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Action EmptyDelegate = delegate () { };
        public OpenFileDialog fileDialog { get; set; }
        public MainWindow()
        {
            InitializeComponent();

        }
        //on click -
        // open the file dialog box
        //parse the data (most of this operation is similar to the console version)
        //output the data to the datagrid
        //output the data to the graph
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".csv"; // Required file extension 
            fileDialog.Filter = "CSV Files|*.csv";
            fileDialog.ShowDialog();

            if (fileDialog.FileName.EndsWith(".csv"))
            {
                //var fileName = fileDialog.FileName;
                fileBox.Text = fileDialog.FileName;
                try
                {
                    Stream fileStream = fileDialog.OpenFile();
                    //var path = "C:/Users/adria/source/repos/AutoneticsChallenge/Files/Code Test Eight Round.csv"; //insert the file path here
                    // call the parse points method
                    Point[] filePoints = ParsePoints(fileStream).ToArray();

                    //Instantiate a PeakAndValleyFinder object, gibing it our array of point objects
                    PeakAndValleyFinder peakAndValleyFinder = new PeakAndValleyFinder(filePoints);

                    //call the DeterminePeaks and DetermineValleys methods (see definition in PeakAndValleyFinder.cs)
                    IEnumerable<Point> peaks = peakAndValleyFinder.DeterminePeaks();
                    IEnumerable<Point> valleys = peakAndValleyFinder.DetermineValleys();

                    // concat and order the two Ienumerables and ToList them for console output
                    var peaksAndValleys = peaks.Concat(valleys).OrderBy(s => s.X).ToList();
                    //set properties for datagrid.
                    CollectionViewSource itemCollectionViewSource;
                    itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
                    itemCollectionViewSource.Source = peaksAndValleys;
                    //get the size of the arrays for the chart, scottplot needs doubles so i can't just use my List.
                    var arraySize = peaksAndValleys.Count();
                    //initialize some doubles for the graph
                    double[] dataX = new double[arraySize];
                    double[] dataY = new double[arraySize];
                    //loop over adding the values to the arrays for the graph.
                    for (int i = 0; i < arraySize; i++)
                    {
                        dataX[i] = peaksAndValleys[i].X;
                        dataY[i] = peaksAndValleys[i].Y;
                    }
                    wpfPlot1.plt.PlotScatter(dataX, dataY);
                    wpfPlot1.Render();
                    fileStream.Close();
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    fileBox.Text = "File is open in another program. Please close any other copies of the file and try again";
                }
            }
        }
        //Parse the dataset
        private static IEnumerable<Point> ParsePoints(Stream file)
        {
            //instantiate a StreamReader to read the lines of the CSV
            using (StreamReader reader = new StreamReader(file))
            {
                
                //keep going until we reach the end
                while (!reader.EndOfStream)
                {
                    
                    //in each loop, we read the next line, and instantiate a new point object
                    var dataLine = reader.ReadLine();
                    var isNumber = false;
                    
                    //here we determine when the actual numbers start in the csv by reading lines and trying to parse them as doubles
                    //if the parse succeeds we can continue on.
                    while (!isNumber)
                    {
                        try
                        {
                            double.Parse(dataLine.Split(',')[0]);
                            isNumber = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            dataLine = reader.ReadLine();
                        }
                    }
                    
                    Point point = new Point()
                    {
                        //split the two colums parse the strings into doubles and set the props of the point object
                        X = double.Parse(dataLine.Split(',')[0]),
                        Y = double.Parse(dataLine.Split(',')[1])
                    };
                    //return the point object
                    yield return point;
                }
            }
        }
    }
}
