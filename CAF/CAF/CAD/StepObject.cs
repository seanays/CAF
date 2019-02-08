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
            Parts = new List<StepPart>();

            InitHeader();
            InitFooter();
        }

        public void AddPart(StepPart part)
        {
            Parts.Add(part);
        }
        private void InitFooter()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"END-ISO-10303-21;");
            Footer = sb.ToString();
            Data = new List<StepFileLineObject>();
        }

        public string Header { get; set; }
        public List<StepFileLineObject> Data { get; set; }
        public string Footer { get; set; }
        public string ApplicationString { get; set; }
        public string GeneratorString { get; set; }
        public string FileNameString { get; set; }

        public List<StepPart> Parts { get; set; }

        private IDGenerator idGenerator;
        private void InitHeader()
        {
            ApplicationString = "HOD";


            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ISO-10303-21;");
            sb.AppendLine($"HEADER;");
            sb.AppendLine($"FILE_DESCRIPTION (( 'STEP AP214' ),");
            sb.AppendLine($"    '1' );");
            sb.AppendLine($"FILE_NAME ('{FileNameString}',");
            sb.AppendLine($"    '2019-02-05T05:03:52',");
            sb.AppendLine($"    ( '' ),");
            sb.AppendLine($"    ( '' ),");
            sb.AppendLine($"    '{GeneratorString}'");
            sb.AppendLine($"    '{ApplicationString}',");
            sb.AppendLine($"    '' );");
            sb.AppendLine($"FILE_SCHEMA (( 'AUTOMOTIVE_DESIGN' ));");
            sb.AppendLine($"ENDSEC;");

            Header = sb.ToString();

        }
        //traverse nodes and assign IDs before emitting
        public void GenerateIDs()
        {
            idGenerator = new IDGenerator();
            foreach (StepPart stepPart in Parts)
            {
                foreach (StepLineObject stepLineObject in stepPart.PartLines)
                {
                    stepLineObject.SetID(idGenerator);
                }
            }
        }

        public string EmitFile()
        {            
            StringBuilder sb = new StringBuilder();

            foreach (StepFileLineObject stepFileLineObject in Data)
            {
                sb.AppendLine(stepFileLineObject.ToString());
            }

            return sb.ToString();

        }
        }
}

public class StepPart
{
    public List<StepLineObject> PartLines { get; set; }

    public StepPart()
    {
        
    }
}

public class IDGenerator
{
    private int partIDCounter;
    public IDGenerator()
    {
        partIDCounter = 1;

    }
    public int GetID()
    {
        return partIDCounter++;
    }
}
public abstract class StepObject
{
    public string EntityString { get; set; }
}
public abstract class StepFileLineObject: StepObject
{
    public const string TrueString = ".T.";
    public const string FalseString = ".F.";


    public const string EnumSideBothString = ".BOTH.";

    public int ID { get; set; }

    public virtual void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
    }

}

public abstract class StepNamedFileLineObject : StepFileLineObject
{
    public string Name { get; set; }
}

public class StepCartesianPoint: StepNamedFileLineObject
{
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public StepCartesianPoint()
    {
        Name = "NONE";
        EntityString = "CARTESIAN_POINT";
    }

}

public class StepVertexPoint : StepNamedFileLineObject
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

public class StepDirection : StepNamedFileLineObject
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

public class StepVector : StepNamedFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Orientation.SetID(idGenerator);
    }

}

public class StepOrientedEdge : StepNamedFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        EdgeElement.SetID(idGenerator);
    }

}

public class StepLineObject : StepNamedFileLineObject
{
    public StepCartesianPoint Pt { get; set; }
    public StepVector Dir { get; set; }


    public StepLineObject()
    {
        EntityString = "LINE";
        Name = "NONE";
    }


    public override string ToString()
    {        
        return $"#{ID} = {EntityString} ( '{Name}', #{Pt.ID},#{Dir.ID});";
    }


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Pt.SetID(idGenerator);
        Dir.SetID(idGenerator);
    }

}

public class StepEdgeCurve : StepNamedFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        EdgeStart.SetID(idGenerator);
        EdgeEnd.SetID(idGenerator);
        EdgeGeometry.SetID(idGenerator);
    }
}

public class StepEdgeLoop : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        foreach (StepOrientedEdge stepOrientedEdge in EdgeList)
        {
            stepOrientedEdge.SetID(idGenerator);
            
        }
    }
}

public class StepAxis2Placement : StepNamedFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Location.SetID(idGenerator);
        Axis.SetID(idGenerator);
        RefDirection.SetID(idGenerator);
    }

}

public class StepFillAreaStyle : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        FillStyle.SetID(idGenerator);
        
    }

}

public class StepSurfaceFillAreaStyle : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        FillArea.SetID(idGenerator);

    }
}



public class StepColorRGB : StepNamedFileLineObject
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

public class StepSurfaceStyleUsage : StepFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Style.SetID(idGenerator);

    }
}

public class StepSurfaceSideStyle : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Styles.SetID(idGenerator);

    }
}



public class StepCylindricalSurface : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Position.SetID(idGenerator);

    }
}

public class StepAdvancedFace : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Bounds.SetID(idGenerator);
        FaceGeometry.SetID(idGenerator);

    }
}

public class StepFaceOuterBound : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Bound.SetID(idGenerator);        

    }
}

public class StepPlane : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Position.SetID(idGenerator);

    }

}



public class StepApplicationProtocolDefinition : StepFileLineObject
{
    public StepApplicationContext Application { get; set; }
    public StepApplicationProtocolDefinition()
    {
        EntityString = "APPLICATION_PROTOCOL_DEFINITION";
    }
    public override string ToString()
    {

        return $"#{ID} = {EntityString}( 'draft international standard', 'automotive_design', 2000, #{Application.ID} ) ;";
    }


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Application.SetID(idGenerator);

    }

}

public class StepUncertaintyMeasureWithUnit : StepFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Unit.SetID(idGenerator);

    }
}

public class StepUnit: StepFileLineObject
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


public class StepGeometricRepresentationGroupContext : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Uncertainty.SetID(idGenerator);
        LengthUnit.SetID(idGenerator);
        PlaneAngleUnit.SetID(idGenerator);
        SolidAngleUnit.SetID(idGenerator);

    }
}

public class StepAdvancedBrepShapeRepresentation : StepNamedFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Item1.SetID(idGenerator);
        Item2.SetID(idGenerator);
        Context.SetID(idGenerator);
        
    }
}

public class StepManifoldSolidBrep : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        Outer.SetID(idGenerator);
     
    }
}

public class StepClosedShell : StepNamedFileLineObject
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



    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();
        foreach (StepAdvancedFace stepAdvancedFace in CfsFaces)
        {
            stepAdvancedFace.SetID(idGenerator);
        }
        

    }
}

public class StepMechanicalDesignGeometricPresentationRepresentation : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Item.SetID(idGenerator);
        Context.SetID(idGenerator);

    }
}

public class StepStyledItem : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Style.SetID(idGenerator);
        Item.SetID(idGenerator);

    }
}

public class StepPresentationStyleAssignment : StepFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Style.SetID(idGenerator);
        
    }
}

public class StepApplicationContext : StepFileLineObject
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

public class StepPresentationLayerAssignment : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        AssignedItem.SetID(idGenerator);

    }
}


public class StepProduct : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        FrameOfReference.SetID(idGenerator);

    }
}

public class StepProductContext : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        FrameOfReference.SetID(idGenerator);

    }
}

public class StepProductDefinitionShape : StepNamedFileLineObject
{
    public string Description { get; set; }
    public StepProductDefinition Definition { get; set; }

    public StepProductDefinitionShape()
    {
        EntityString = "PRODUCT_DEFINITION_SHAPE";
        Name = "NONE";
        Description = "NONE";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} ( '{Name}','{Description}', #{Definition.ID});";
    }


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Definition.SetID(idGenerator);

    }
}

public class StepProductDefinition : StepFileLineObject
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

    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Formation.SetID(idGenerator);
        FrameOfReference.SetID(idGenerator);

    }
}


public class StepProductDefinitionFormationWithSpecifiedSource : StepFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        OfProduct.SetID(idGenerator);
        

    }

}

public class StepShapeDefinitionRepresentation : StepLineObject
{
    public StepProductDefinitionShape Definition { get; set; }
    public StepAdvancedBrepShapeRepresentation  UsedRepresentation { get; set; }

    public StepShapeDefinitionRepresentation()
    {
        EntityString = "SHAPE_DEFINITION_REPRESENTATION";
    }

    public override string ToString()
    {
        return $"#{ID} = {EntityString} (  #{Definition.ID}, #{UsedRepresentation.ID});";
    }


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        Definition.SetID(idGenerator);
        UsedRepresentation.SetID(idGenerator);


    }
}
public class StepProductDefinitionContext : StepNamedFileLineObject
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


    public override void SetID(IDGenerator idGenerator)
    {
        ID = idGenerator.GetID();

        FrameOfReference.SetID(idGenerator);


    }
}