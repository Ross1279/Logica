namespace LogicalAnalyzer
{
    public class Expressions
    {
        public Expressions(string left, string right)
        {
            Left = left;
            Right = right;
        }
        public string Left { get; set; }
        public string Right { get; set; }
        private bool isConsequence = true;

        public bool IsConsequence(bool left, bool right)
        {
            if (!isConsequence) return false;

            if (left)
            {
                if (right)
                {
                    isConsequence = true;
                    return true;
                }
                else
                {
                    isConsequence = false;
                    return false;
                }
            }

            return true;
        }

        public bool Result()
        {
            return isConsequence;
        }
    }
}
