namespace Robot
{
    public class Color
    {
        public static Color BLACK = new Color(0, 0, 0);
        public static Color RED = new Color(255, 0, 0);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color BLUE = new Color(0, 0, 255);
        public static Color WHITE = new Color(255, 255, 255);
        public int R;
        public int G;
        public int B;

        public Color(int R, int G, int B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }
    }
}
