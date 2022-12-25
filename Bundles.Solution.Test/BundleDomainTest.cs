namespace Bundles.Solution.Test;

public class BundleDomainTest
{
    [Fact]
    public void Create_bundle_with_empty_name_should_throw_exception()
    {
        //Arrange
        string name = " ";
        string expectedMssage = "Name shouldn't be null or empty";

        //Act - Assert
        var exception = Assert.Throws<ArgumentException>(() => new Bundle(name, true, 1));
        Assert.StartsWith(expectedMssage, exception.Message);
        Assert.Equal(nameof(name), exception.ParamName);
    }

    [Fact]
    public void Create_bundle_with_number_of_units_equal_zero_should_throw_exception()
    {
        //Arrange
        int numberOfUnits = 0;
        string expectedMssage = "Number of units shouldn't be less then 1";

        //Act - Assert
        var exception = Assert.Throws<ArgumentException>(() => new Bundle("Seat", true, numberOfUnits));
        Assert.StartsWith(expectedMssage, exception.Message);
        Assert.Equal(nameof(numberOfUnits), exception.ParamName);
    }

    [Fact]
    public void Create_bundle_as_non_raw_material_with_no_bundle_parts_should_throw_exception()
    {
        //Arrange
        List<Bundle> bundleParts = default!;
        string expectedMssage = "Bundle parts shouldn't be less then 1 for non raw material";

        //Act - Assert
        var exception = Assert.Throws<ArgumentException>(() => new Bundle("Seat", false, 1, bundleParts));
        Assert.StartsWith(expectedMssage, exception.Message);
        Assert.Equal(nameof(bundleParts), exception.ParamName);
    }

    [Fact]
    public void Create_bundle_as_raw_material_should_success()
    {
        //Arrange - Act
        var frame = new Bundle("Frame", true, 1);

        //Assert
        Assert.NotNull(frame);
        Assert.Empty(frame.BundleParts);
    }

    [Fact]
    public void Create_bundle_as_non_raw_material_should_success()
    {
        //Arrange
        var frame = new Bundle("Frame", true, 1);
        var tube = new Bundle("Tube", true, 1);

        //Act
        var wheel = new Bundle("Wheel", false, 2, new List<Bundle> { frame, tube });

        //Assert
        Assert.NotNull(wheel);
        Assert.NotNull(wheel.BundleParts);
        Assert.NotEmpty(wheel.BundleParts);
    }

    [Fact]
    public void Raw_material_inventory_should_success()
    {
        //Arrange
        Bundle bike = CreateBikeBundle();
        var expectedResult = new List<(string Name, int Count)>
        {
            ("Seat",1),
            ("Pedal",2),
            ("Frame",2),
            ("Tube",2),
        };

        //Act
        List<(string Name, int Count)> rawMaterials = bike.RawMaterialInventory();

        //Assert
        Assert.Equal(expectedResult, rawMaterials);
    }

    [Fact]
    public void Get_maximum_number_of_finished_bundles_should_success()
    {
        //Arrange
        Bundle bike = CreateBikeBundle();

        var rawMaterialsStock = new List<(string Name, int Count)>
        {
            ("Seat",50),
            ("Pedal",60),
            ("Frame",60),
            ("Tube",35),
        };

        int expectedResult = 17;

        //Act
        var result = bike.GetMaximumNumberOfFinishedBundles(rawMaterialsStock);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    private static Bundle CreateBikeBundle()
    {
        var seat = new Bundle("Seat", true, 1);
        var pedal = new Bundle("Pedal", true, 2);

        var frame = new Bundle("Frame", true, 1);
        var tube = new Bundle("Tube", true, 1);
        var wheel = new Bundle("Wheel", false, 2, new List<Bundle> { frame, tube });

        var bike = new Bundle("Bike", false, 1, new List<Bundle> { seat, pedal, wheel });
        return bike;
    }
}
