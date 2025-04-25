using Proj4Net.Core.Utility;

namespace Proj4Net.Core.Datum.Grids
{
    public struct PhiLambda
    {
        public double Phi;
        public double Lambda;

        public static PhiLambda operator +(PhiLambda lhs, PhiLambda rhs)
        {
            return new PhiLambda {Phi = lhs.Phi + rhs.Phi, Lambda = lhs.Lambda + rhs.Lambda};
        }

        public static PhiLambda operator -(PhiLambda lhs, PhiLambda rhs)
        {
            return new PhiLambda {Phi = lhs.Phi - rhs.Phi, Lambda = lhs.Lambda - rhs.Lambda};
        }

        public static PhiLambda DegreesToRadians(PhiLambda plInDegrees)
        {
            return new PhiLambda
                {
                    Lambda = ProjectionMath.ToRadians(plInDegrees.Lambda),
                    Phi = ProjectionMath.ToRadians(plInDegrees.Phi)
                };
        }
        public static PhiLambda SecondsToDegrees(PhiLambda plInDegrees)
        {
            return new PhiLambda
            {
                Lambda = ProjectionMath.SecondsToDegrees(plInDegrees.Lambda),
                Phi = ProjectionMath.SecondsToDegrees(plInDegrees.Phi)
            };
        }

        public static PhiLambda ArcSecondsToRadians(PhiLambda plInDegrees)
        {
            return new PhiLambda
                {
                    Lambda = ProjectionMath.ArcSecondsToRadians(plInDegrees.Lambda),
                    Phi = ProjectionMath.ArcSecondsToRadians(plInDegrees.Phi)
                };
        }
        public static PhiLambda ArcSecondsToRadians(double phi, double lambda)
        {
            return new PhiLambda
                {
                    Lambda = ProjectionMath.ArcSecondsToRadians(lambda),
                    Phi = ProjectionMath.ArcSecondsToRadians(phi)
                };
        }
        public static PhiLambda ArcMicroSecondsToRadians(PhiLambda plInDegrees)
        {
            return new PhiLambda
                {
                    Lambda = ProjectionMath.ArcMicroSecondsToRadians(plInDegrees.Lambda),
                    Phi = ProjectionMath.ArcMicroSecondsToRadians(plInDegrees.Phi)
                };
        }
        public static PhiLambda ArcMicroSecondsToRadians(double phi, double lambda)
        {
            return new PhiLambda
                {
                    Lambda = ProjectionMath.ArcMicroSecondsToRadians(lambda),
                    Phi = ProjectionMath.ArcMicroSecondsToRadians(phi)
                };
        }
        public PhiLambda Times(int numPhis, int numLambdas)
        {
            return new PhiLambda {Lambda = numLambdas*numLambdas, Phi = Phi*numPhis};
        }
    }
}