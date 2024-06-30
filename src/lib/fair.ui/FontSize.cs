namespace fair.ui
{
    public class FontSize
    {
        private string size;

        public FontSize(string size)
        {
            this.size = size;

        }

        /// <summary>
        /// 3rem
        /// </summary>
        public static FontSize Size1 = new FontSize("is-size-1"); //3rem
        /// <summary>
        /// 2.5rem
        /// </summary>
        public static FontSize Size2 = new FontSize("is-size-2");
        /// <summary>
        /// 2rem
        /// </summary>
        public static FontSize Size3 = new FontSize("is-size-3");

        /// <summary>
        /// 1.5rem
        /// </summary>
        public static FontSize Size4 = new FontSize("is-size-4");
        /// <summary>
        /// 1.25rem
        /// </summary>
        public static FontSize Size5 = new FontSize("is-size-5");
        /// <summary>
        /// 1rem
        /// </summary>
        public static FontSize Size6 = new FontSize("is-size-6");


        /// <summary>
        /// 0.75rem
        /// </summary>
        public static FontSize Size7 = new FontSize("is-size-7"); //0.75rem




        public override string ToString()
        {

            return size;
        }
    }
}
