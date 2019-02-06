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
        public StepObject()
        {
            InitHeader();
            InitFooter();
        }

        private void InitFooter()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"END-ISO-10303-21;");
            Footer = sb.ToString();

        }

        public string Header { get; set; }
        public string  Data { get; set; }
        public string Footer { get; set; }
        
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

public abstract class StepObject
{
    public string EntityString { get; set; }
}
public abstract class StepLineObject: StepObject
{
    public const string TrueString = ".T.";
    public const string FalseString = ".F.";


    public const string EnumSideBothString = ".BOTH.";

    public int ID { get; set; }

    
    
}

public class StepNamedLineObject : StepLineObject
{
    public string Name { get; set; }
}

public class StepCartesianPoint: StepNamedLineObject
{
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public StepCartesianPoint()
    {
        Name = "NONE";
        EntityString = "CARTESIAN_POINT";
    }
    public override string ToString()
    {
                    
        return $"#{ID} = {EntityString} ( '{Name}', ({X},{Y},{Z}) );";
    }
}

public class StepVertexPoint : StepNamedLineObject
{
    public StepCartesianPoint Point { get; set; }
    
    public StepVertexPoint()
    {
        Name = "NONE";
        EntityString = "VERTEX_POINT";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', #{Point.ID} );";
    }
}

public class StepDirection : StepNamedLineObject
{
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public StepDirection()
    {
        Name = "NONE";
        EntityString = "DIRECTION";
    }

    public override string ToString()
    {

        return $"#{ID} = {EntityString} ( '{Name}', ({X},{Y},{Z}) );";
    }

}

public class StepVector : StepNamedLineObject
{
    
    public StepDirection Orientation { get; set; }
    public double Magnitude { get; set; }
    
    public StepVector()
    {
        Name = "NONE";
        EntityString = "VECTOR";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', (#{Orientation.ID},{Magnitude}) );";
    }

}

public class StepOrientedEdge : StepNamedLineObject
{
    
    public StepEdgeCurve EdgeElement { get; set; }
    public bool Orientation { get; set; }

    public StepOrientedEdge()
    {
        Name = "NONE";
        EntityString = "ORIENTED_EDGE";
    }

    public override string ToString()
    {
        string orientationString = Orientation ? TrueString : FalseString;
        
        return $"#{ID} = {EntityString} ( '{Name}', *, *, (#{EdgeElement.ID},{orientationString}) );";
    }

}

public class StepEdgeCurve : StepNamedLineObject
{
    
    public StepVertexPoint EdgeStart { get; set; }
    public StepVertexPoint EdgeEnd { get; set; }
    public StepLineObject EdgeGeometry { get; set; }


    public StepEdgeCurve()
    {
        Name = "NONE";
        EntityString = "EDGE_CURVE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', #{EdgeStart.ID}, #{EdgeEnd}, #{EdgeGeometry}, .T. );";
    }

}

public class StepEdgeLoop : StepNamedLineObject
{
    
    public ObservableCollection<StepOrientedEdge> EdgeList { get; set; }
    
    public StepEdgeLoop()
    {
        Name = "NONE";
        EntityString = "EDGE_LOOP";
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
        return $"#{ID} = {EntityString} ( '{Name}', ({sb.ToString()}) );";
    }

}

public class StepAxis2Placement : StepNamedLineObject
{    
    public StepCartesianPoint Location { get; set; }
    public StepDirection Axis { get; set; }
    public StepDirection RefDirection { get; set; }

    public StepAxis2Placement()
    {
        EntityString = "AXIS2_PLACEMENT_3D";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', #{Location.ID}, #{Axis.ID}, #{RefDirection.ID} );";
    }

}

public class StepFillAreaStyle : StepNamedLineObject
{    

    public StepColorRGB FillStyle { get; set; }

    public StepFillAreaStyle()
    {
        EntityString = "FILL_AREA_STYLE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', ( #{FillStyle.ID}) );";
    }

}

public class StepSurfaceFillAreaStyle : StepNamedLineObject
{    

    public StepFillAreaStyle FillArea { get; set; }

    public StepSurfaceFillAreaStyle()
    {
        EntityString = "SURFACE_STYLE_FILL_AREA";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( #{FillArea.ID} );";
    }

}



public class StepColorRGB : StepNamedLineObject
{
    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }

    public StepColorRGB()
    {
        Name = string.Empty;
        EntityString = "COLOUR_RGB";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', {R},{G},{B} );";
    }

}

public class StepSurfaceStyleUsage : StepLineObject
{    

    public StepSurfaceSideStyle Style { get; set; }

    public StepSurfaceStyleUsage()
    {
        EntityString = "SURFACE_STYLE_USAGE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString}  ( {EnumSideBothString}, #{Style.ID});";
    }
}

public class StepSurfaceSideStyle : StepNamedLineObject
{    

    public StepSurfaceFillAreaStyle Styles { get; set; }

    public StepSurfaceSideStyle()
    {
        Name = string.Empty;
        EntityString = "SURFACE_SIDE_STYLE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( {EnumSideBothString}, (#{Styles.ID}) );";
    }
}



public class StepCylindricalSurface : StepNamedLineObject
{    

    public StepAxis2Placement Position { get; set; }

    public double Radius { get; set; }

    public StepCylindricalSurface()
    {
        Name = "NONE";
        EntityString = "CYLINDRICAL_SURFACE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' , #{Position.ID}, {Radius});";
    }
}

public class StepAdvancedFace : StepNamedLineObject
{    

    //this should be a list at some point
    public StepFaceOuterBound Bounds { get; set; }

    public StepPlane FaceGeometry { get; set; }

    public bool SameSense { get; set; }

    public StepAdvancedFace()
    {
        Name = "NONE";
        EntityString = "ADVANCED_FACE";
    }

    public override string ToString()
    {
        string sameSenseStr = SameSense ? TrueString : FalseString;

        return $"#{ID} = {EntityString} ( '{Name}' ,( #{Bounds.ID}), #{FaceGeometry.ID}, {sameSenseStr});";
    }


}

public class StepFaceOuterBound : StepNamedLineObject
{    
    public StepEdgeLoop Bound { get; set; }
    public bool Orientation { get; set; }

    public StepFaceOuterBound()
    {
        Name = "NONE";
        EntityString = "FACE_OUTER_BOUND";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' , #{Bound.ID}, {TrueString});";
    }


}

public class StepPlane : StepNamedLineObject
{    

    public StepAxis2Placement Position { get; set; }


    public StepPlane()
    {
        Name = "NONE";
        EntityString = "PLANE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' , #{Position.ID} );";
    }


}



public class StepApplicationProtocolDefinition : StepLineObject
{
    public StepApplicationProtocolDefinition()
    {
        EntityString = "APPLICATION_PROTOCOL_DEFINITION";
    }
    public override string ToString()
    {

        return $"#{ID} = {EntityString}( 'draft international standard', 'automotive_design', 1998, #108 ) ;";
    }
    
}

public class StepUncertaintyMeasureWithUnit : StepLineObject
{
    public StepLengthUnit Unit { get; set; }
    
    public StepUncertaintyMeasureWithUnit()
    {
        EntityString = "UNCERTAINTY_MEASURE_WITH_UNIT";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} (LENGTH_MEASURE( 1.000000000000000082E-05 ), #{Unit.ID}, 'distance_accuracy_value', 'NONE');";
    }

}

public class StepUnit: StepLineObject
{
}

public class StepLengthUnit : StepUnit
{

    public override string ToString()
    {
        return $"#{ID} = ( LENGTH_UNIT ( ) NAMED_UNIT ( * ) SI_UNIT ( $, .METRE. ) );";
    }
}

public class StepPlaneAngleUnit : StepUnit
{

    public override string ToString()
    {
        return $"#{ID} = ( NAMED_UNIT ( * ) PLANE_ANGLE_UNIT ( ) SI_UNIT ( $, .RADIAN. ) );";
    }
}


public class StepSolidAngleUnit : StepUnit
{

    public override string ToString()
    {
        return $"#{ID} = ( NAMED_UNIT ( * ) SI_UNIT ( $, .STERADIAN. ) SOLID_ANGLE_UNIT ( ) );";
    }
}


public class StepGeometricRepresentationGroupContext : StepNamedLineObject
{
    
    public StepUncertaintyMeasureWithUnit Uncertainty { get; set; }
    public StepLengthUnit LengthUnit { get; set; }
    public StepPlaneAngleUnit PlaneAngleUnit { get; set; }
    public StepSolidAngleUnit SolidAngleUnit { get; set; }

    public StepGeometricRepresentationGroupContext()
    {
        Name = "distance_accuracy_value";
    }
   

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"#{ID} = ( GEOMETRIC_REPRESENTATION_CONTEXT ( 3 ) ");
        sb.Append($" GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT ( ( #{Uncertainty.ID} ) )");
        sb.Append($" GLOBAL_UNIT_ASSIGNED_CONTEXT ( ( #{LengthUnit.ID}, #{PlaneAngleUnit.ID}, #{SolidAngleUnit.ID} ) )");
        sb.Append($" REPRESENTATION_CONTEXT ( 'NONE', 'WORKASPACE' ) );");

        return sb.ToString();
    }

}

public class StepAdvancedBrepShapeRepresentation : StepNamedLineObject
{
    public string Name { get; set; }

    public StepManifoldSolidBrep Item1 { get; set; }
    public StepAxis2Placement Item2 { get; set; }
    public StepGeometricRepresentationGroupContext Context { get; set; }
    public StepAdvancedBrepShapeRepresentation()
    {
        EntityString = "ADVANCED_BREP_SHAPE_REPRESENTATION";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' ,(#{Item1.ID}, #{Item2.ID}), #{Context.ID} );";
    }
}

public class StepManifoldSolidBrep : StepNamedLineObject
{
    public StepClosedShell Outer { get; set; }

    public StepManifoldSolidBrep()
    {
        EntityString = "MANIFOLD_SOLID_BREP";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' , #{Outer.ID} );";
    }

}

public class StepClosedShell : StepNamedLineObject
{    
    public ObservableCollection<StepAdvancedFace> CfsFaces { get; set; }

    public StepClosedShell()
    {
        EntityString = "CLOSED_SHELL";
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        bool first = true;

        foreach (StepAdvancedFace stepAdvancedFace in CfsFaces)
        {
            if (first)
            {
                first = false;
                sb.Append($"#{stepAdvancedFace.ID}");
            }
            else
            {
                sb.Append($", #{stepAdvancedFace.ID}");
            }
        }
        return $"#{ID} = {EntityString} ( '{Name}' , ({sb}) );";
    }

}

public class StepMechanicalDesignGeometricPresentationRepresentation : StepNamedLineObject
{
    public StepStyledItem Item { get; set; }
    public StepGeometricRepresentationGroupContext Context { get; set; }

    public StepMechanicalDesignGeometricPresentationRepresentation()
    {
        EntityString = "MECHANICAL_DESIGN_GEOMETRIC_PRESENTATION_REPRESENTATION";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}' ,(#{Item.ID}), #{Context.ID} );";
    }
}

public class StepStyledItem : StepNamedLineObject
{
    public StepPresentationStyleAssignment Style { get; set; }
    public StepManifoldSolidBrep Item { get; set; }

    public StepStyledItem()
    {
        EntityString = "STYLED_ITEM";

    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', (#{Style.ID}), #{Item.ID} );";
    }

}

public class StepPresentationStyleAssignment : StepLineObject
{
    public StepSurfaceStyleUsage Style { get; set; }

    public StepPresentationStyleAssignment()
    {
        EntityString = "PRESENTATION_STYLE_ASSIGNMENT";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( (#{Style.ID}) );";
    }

}

public class StepApplicationContext : StepLineObject
{
    public StepApplicationContext()
    {
        EntityString = "APPLICATION_CONTEXT";

    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( 'automotive_design' );";
    }
}

public class StepPresentationLayerAssignment : StepNamedLineObject
{
    public string Description { get; set; }

    public StepStyledItem AssignedItem { get; set; }

    public StepPresentationLayerAssignment()
    {
        EntityString = "PRESENTATION_LAYER_ASSIGNMENT";

    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', '{Description}', (#{AssignedItem.ID}));";
    }
}


public class StepProduct : StepNamedLineObject
{
    public string StringID { get; set; }
    public string Description { get; set; }
    public StepProductContext FrameOfReference { get; set; }

    public StepProduct()
    {
        EntityString = "PRODUCT";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{StringID}', '{Name}', '{Description}', (#{FrameOfReference.ID}));";
    }
}

public class StepProductContext : StepNamedLineObject
{
    public StepApplicationContext FrameOfReference { get; set; }

    public StepProductContext()
    {
        Name = "NONE";
        EntityString = "PRODUCT_CONTEXT";
    }


    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', #{FrameOfReference.ID}, 'mechanical');";
    }

}

public class StepProductDefinitonShape : StepNamedLineObject
{
    public string Description { get; set; }
    public StepProductDefinition Definition { get; set; }

    public StepProductDefinitonShape()
    {
        EntityString = "PRODUCT_DEFINITION_SHAPE";
        Name = "NONE";
        Description = "NONE";
    }
    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}','{Description}', #{Definition.ID});";
    }

}

public class StepProductDefinition : StepLineObject
{
    public string StringID { get; set; }
    public string Description { get; set; }

    public StepProductDefinitionFormationWithSpecifiedSource Formation { get; set; }
    public StepProductDefinitionContext FrameOfReference  { get; set; }


    public StepProductDefinition()
    {
        EntityString = "PRODUCT_DEFINITION";
        StringID = "UNKNOWN";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{StringID}','{Description}', #{Formation.ID}, #{FrameOfReference.ID});";
    }

}


public class StepProductDefinitionFormationWithSpecifiedSource : StepLineObject
{
    public string StringID { get; set; }
    public string Description { get; set; }
    public StepProduct OfProduct { get; set; }

    public StepProductDefinitionFormationWithSpecifiedSource()
    {
        EntityString = "PRODUCT_DEFINITION_FORMATION_WITH_SPECIFIED_SOURCE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( 'ANY','{Description}', #{OfProduct.ID}, .NOT_KNOWN.);";
    }

}

public class StepProductDefinitionContext : StepNamedLineObject
{
    public string LifeCycleStage { get; set; }
    public StepApplicationContext FrameOfReference { get; set; }

    public StepProductDefinitionContext()
    {
        Name = "detailed design";
        EntityString = "PRODUCT_DEFINITION_CONTEXT";
        LifeCycleStage = "design";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}', #{FrameOfReference.ID}, '{LifeCycleStage}');";
    }

}