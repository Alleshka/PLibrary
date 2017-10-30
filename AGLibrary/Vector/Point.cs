namespace AGLibrary.Vector
{
    public class Point : Vector<double>
    {
        public Point(double a, double b) : base(new double[2] { a, b })
        {

        }

        public double X
        {
            get => this[0];
            set => this[0] = value;
        }

        public double Y
        {
            get => this[1];
            set => this[1] = value;
        }
    }
}