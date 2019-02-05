using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CAF.CAD
{
    public class StepObject
    {
        
        public string Header { get; set; }
        public string  Data { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        private void InitHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ISO-10303-21;");
            sb.AppendLine($"HEADER;");
            sb.AppendLine($"FILE_DESCRIPTION (( 'STEP AP214' ),");
            sb.AppendLine($"    '1' );");
            sb.AppendLine($"FILE_NAME ('Wedge.STEP',");
            sb.AppendLine($"    '2019-02-05T05:03:52',");
            sb.AppendLine($"    ( '' ),");
            sb.AppendLine($"    ( '' ),");
            sb.AppendLine($"    'SwSTEP 2.0'");
            sb.AppendLine($"    'SolidWorks 2018',");
            sb.AppendLine($"    '' );");
            sb.AppendLine($"FILE_SCHEMA (( 'AUTOMOTIVE_DESIGN' ));");
            sb.AppendLine($"ENDSEC;");

            Header = sb.ToString();

        }

        public int LineCounter { get; private set; }

        public int GetLineID()
        {
            return LineCounter++;
        }


    }
}

public abstract class StepLineObject
{
    public const string TrueString = ".T.";
    public const string FalseString = ".F.";

    public int ID { get; set; }

    
    
}

public class StepCartesianPoint: StepLineObject
{
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public StepCartesianPoint()
    {
        Name = "NONE";
    }
    public override string ToString()
    {
                    
        return $"#{ID} = CARTESIAN_POINT ( '{Name}', ({X},{Y},{Z}) );";
    }
}

public class StepVertexPoint : StepLineObject
{
    public string Name { get; set; }
    public StepCartesianPoint Point { get; set; }
    
    public StepVertexPoint()
    {
        Name = "NONE";
    }

    public override string ToString()
    {
        return $"#{ID} = VERTEX_POINT ( '{Name}', #{Point.ID} );";
    }
}

public class StepDirection : StepLineObject
{
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public StepDirection()
    {
        Name = "NONE";
    }

    public override string ToString()
    {

        return $"#{ID} = DIRECTION ( '{Name}', ({X},{Y},{Z}) );";
    }

}

public class StepVector : StepLineObject
{
    public string Name { get; set; }
    public StepDirection Orientation { get; set; }
    public double Magnitude { get; set; }
    
    public StepVector()
    {
        Name = "NONE";
    }

    public override string ToString()
    {
        return $"#{ID} = VECTOR ( '{Name}', (#{Orientation.ID},{Magnitude}) );";
    }

}

public class StepOrientedEdge : StepLineObject
{
    public string Name { get; set; }
    public StepEdgeCurve EdgeElement { get; set; }
    public bool Orientation { get; set; }

    public StepOrientedEdge()
    {
        Name = "NONE";
    }

    public override string ToString()
    {
        string orientationString = Orientation ? TrueString : FalseString;
        
        return $"#{ID} = ORIENTED_EDGE ( '{Name}', *, *, (#{EdgeElement.ID},{orientationString}) );";
    }

}

public class StepEdgeCurve : StepLineObject
{
    public string Name { get; set; }
    public StepVertexPoint EdgeStart { get; set; }
    public StepVertexPoint EdgeEnd { get; set; }
    public StepLineObject EdgeGeometry { get; set; }


    public StepEdgeCurve()
    {
        Name = "NONE";
    }

    public override string ToString()
    {
        return $"#{ID} = EDGE_CURVE ( '{Name}', #{EdgeStart.ID}, #{EdgeEnd}, #{EdgeGeometry}, .T. );";
    }

}

public class StepEdgeLoop : StepLineObject
{
    public string Name { get; set; }
    public ObservableCollection<StepOrientedEdge> EdgeList { get; set; }
    

    public StepEdgeLoop()
    {
        Name = "NONE";
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (StepOrientedEdge stepOrientedEdge in EdgeList)
        {
            if (first)
            {
                sb.Append($"#{stepOrientedEdge.ID}");
                first = false;
            }
            else
            {
                sb.Append($", #{stepOrientedEdge.ID}");
            }
            
        }
        return $"#{ID} = EDGE_LOOP ( '{Name}', ({sb.ToString()}) );";
    }

}

public class StepAxis2Placement : StepLineObject
{
    public string Name { get; set; }

    public StepCartesianPoint Location { get; set; }
    public StepDirection Axis { get; set; }
    public StepDirection RefDirection { get; set; }

    public override string ToString()
    {
        return $"#{ID} = AXIS2_PLACEMENT_3D ( '{Name}', #{Location.ID}, #{Axis.ID}, #{RefDirection.ID} );";
    }

}

public class StepFillAreaStyle : StepLineObject
{
    public string Name { get; set; }

    public StepColorRGB FillStyle { get; set; }

    public override string ToString()
    {
        return $"#{ID} = FILL_AREA_STYLE ( '{Name}', ( #{FillStyle.ID}) );";
    }

}

public class StepColorRGB : StepLineObject
{

    public string Name { get; set; }

    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }

    public StepColorRGB()
    {
        Name = string.Empty;
    }

    public override string ToString()
    {
        return $"#{ID} = COLOUR_RGB ( '{Name}', {R},{G},{B} );";
    }

}

public class StepSurfaceStyleUsage : StepLineObject
{

}