namespace SoftTeach.UndoRedo
{
  using System.Globalization;

  public class ChangeKey<T1, T2>
    {
        private T1 m_One;
        private T2 m_Two;

        public T1 Item1 { get { return this.m_One; } }
        public T2 Item2 { get { return this.m_Two; } }

        public ChangeKey(T1 item1, T2 item2)
        {
            this.m_One = item1;
            this.m_Two = item2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ChangeKey<T1, T2> tuple = obj as ChangeKey<T1, T2>;
            if (tuple == null)
            {
                return false;
            }
            if (object.Equals(this.m_One, tuple.m_One))
            {
                return object.Equals(this.m_Two, tuple.m_Two);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(this.m_One.GetHashCode(), this.m_Two.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Tuple of '{0}', '{1}'", this.m_One, this.m_Two);
        }


        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

    }

    public class ChangeKey<T1, T2, T3>
    {
        private T1 m_One;
        private T2 m_Two;
        private T3 m_Three;

        public T1 Item1 { get { return this.m_One; } }
        public T2 Item2 { get { return this.m_Two; } }
        public T3 Item3 { get { return this.m_Three; } }

        public ChangeKey(T1 item1, T2 item2, T3 item3)
        {
            this.m_One = item1;
            this.m_Two = item2;
            this.m_Three = item3;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ChangeKey<T1, T2, T3> tuple = obj as ChangeKey<T1, T2, T3>;
            if (tuple == null)
            {
                return false;
            }
            if (object.Equals(this.m_One, tuple.m_One))
            {
                if (object.Equals(this.m_Two, tuple.m_Two))
                {
                    return object.Equals(this.m_Three, tuple.m_Three);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(this.m_One.GetHashCode(), CombineHashCodes(this.m_Two.GetHashCode(), this.m_Three.GetHashCode()));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Tuple of '{0}', '{1}', '{2}'", this.m_One, this.m_Two, this.m_Three);
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

    }
}
