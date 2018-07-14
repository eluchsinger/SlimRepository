namespace SlimRepository.EntityFrameworkCore.Test.Data
{
    public class TestObject
    {
        public TestObject(int id = 0, string name = null)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
