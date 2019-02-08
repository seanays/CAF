namespace CAF.CAD
{
    public class CADServices
    {
        public CADServices()
        {
            
        }

        public static void CreateCube(double dimX, double dimY, double dimZ, double cubeX=0, double cubeY = 0, double cubeZ = 0)
        {
            StepPart part = new StepPart();
            StepShapeDefinitionRepresentation shapeRep = new StepShapeDefinitionRepresentation();
            StepProductDefinitionShape defnShape = new StepProductDefinitionShape();
            StepAdvancedBrepShapeRepresentation brepShapeRepresentation = new StepAdvancedBrepShapeRepresentation();

            StepAxis2Placement axis = new StepAxis2Placement();
            SetDefaultAxis(cubeX, cubeY, cubeZ, axis);

            
            //Create base face
            double x1, x2, y1, y2;

            double xPlus=0, xMinus=0;
            double yPlus = 0, yMinus = 0;
            double zPlus = 0, zMinus = 0;

            GeneratePlusMinusValues(dimZ, out zPlus, out zMinus);

            GenerateRectangularFaceVertices(dimX, dimY, out xPlus, out yPlus, out xMinus, out yMinus);

            
            

        }

        private static void SetDefaultAxis(double axisX, double axisY, double axisZ, StepAxis2Placement axis)
        {
            double aDirectionX = 0;
            double aDirectionY = 0;
            double aDirectionZ = 1;

            double refDirectionX = 1;
            double refDirectionY = 0;
            double refDirectionZ = 0;

            SetAxis(axisX, axisY, axisZ, axis, aDirectionX, aDirectionY, aDirectionZ, refDirectionX, refDirectionY, refDirectionZ);
        }

        private static void SetAxis(double axisX, double axisY, double axisZ, StepAxis2Placement axis, double aDirectionX,
            double aDirectionY, double aDirectionZ, double refDirectionX, double refDirectionY, double refDirectionZ)
        {
            StepCartesianPoint point = new StepCartesianPoint();
            SetCartesianPoint(axisX, axisY, axisZ, point);

            StepDirection axisDirection = new StepDirection();
            SetDirection(aDirectionX, aDirectionY, aDirectionZ, axisDirection);

            StepDirection refDirection = new StepDirection();
            SetDirection(refDirectionX, refDirectionY, refDirectionZ, refDirection);

            axis.Location = point;
            axis.Axis = axisDirection;
            axis.RefDirection = refDirection;
        }

        private static void SetCartesianPoint(double x, double y, double z,
            StepCartesianPoint point)
        {
            point.X = x;
            point.Y = y;
            point.Z = z;
        }

        private static void SetDirection(double x, double y, double z,
            StepDirection dir)
        {
            dir.X = x;
            dir.Y = y;
            dir.Z = z;
        }

        private static void GenerateRectangularFaceVertices(double dimX, double dimY, out double xPlus, out double yPlus,
            out double xMinus, out double yMinus)
        {
            GeneratePlusMinusValues(dimX, out xPlus, out xMinus);
            GeneratePlusMinusValues(dimY, out yPlus, out yMinus);

        }

        private static void GeneratePlusMinusValues(double dimension, out double plusValue, out double minusValue)
        {
            plusValue = dimension * 0.5;
            minusValue = -dimension * 0.5;
        }
    }
}