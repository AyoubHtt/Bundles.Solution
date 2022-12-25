namespace Bundles.Solution.BundleAggregate;

public class Bundle
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsRawMaterial { get; private set; }
    public int NumberOfUnits { get; private set; }

    // DDD Patterns comment
    // Using a private collection field, better for DDD Aggregate's encapsulation
    // so Bundles cannot be added from "outside the AggregateRoot" directly to the collection.
    private readonly List<Bundle> _bundleParts = new();
    public IReadOnlyCollection<Bundle> BundleParts => _bundleParts;

    private Bundle() { }

    public Bundle(string name, bool isRawMaterial, int numberOfUnits, List<Bundle> bundleParts = default!)
    {
        // Since we don't have a validation layer I will just put all my validation in the constructor
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name shouldn't be null or empty", nameof(name));
        IsRawMaterial = isRawMaterial;
        NumberOfUnits = numberOfUnits > 0 ? numberOfUnits : throw new ArgumentException("Number of units shouldn't be less then 1", nameof(numberOfUnits));
        if (!isRawMaterial) _bundleParts = bundleParts?.Count > 0 ? bundleParts : throw new ArgumentException("Bundle parts shouldn't be less then 1 for non raw material", nameof(bundleParts)); ;
    }

    public List<(string Name, int Count)> RawMaterialInventory(List<(string name, int count)> rawMaterials = default!, int NumberOfParentUnits = 1)
    {
        rawMaterials ??= new();

        // In case of raw material we inventory the number of units (raw material does not have bundle parts)
        if (IsRawMaterial)
        {
            rawMaterials.Add((Name, NumberOfParentUnits * NumberOfUnits));
            return rawMaterials;
        }

        // In case of non raw material we will go forward to inventory the number of units of bundle parts
        _bundleParts.ForEach(bundlePart => bundlePart.RawMaterialInventory(rawMaterials, NumberOfParentUnits * NumberOfUnits));
        return rawMaterials;
    }

    public int GetMaximumNumberOfFinishedBundles(List<(string Name, int Count)> stockRawMaterials)
    {
        List<(string Name, int Count)> rawMaterials = RawMaterialInventory();

        /// In case of a raw material can be duplicated in the other bundle parts, we can solve that by add the following code lines
        /// rawMaterials = rawMaterials.GroupBy(rawMaterial => rawMaterial.Name)
        ///                            .Select(group => (group.Key, group.Sum(rawMaterial => rawMaterial.Count)))
        ///                            .ToList();

        // To obtain the maximum number of finished bundles is sufficient to incorporate the consumption rate
        // then the prodaction of bundles will stop at the first exhausted raw material 
        var maximumNumberOfFinishedBundles = (from rawMaterial in rawMaterials
                                              join stockRawMaterial in stockRawMaterials on rawMaterial.Name equals stockRawMaterial.Name
                                              select stockRawMaterial.Count/rawMaterial.Count).Min();

        return maximumNumberOfFinishedBundles;
    }
}
