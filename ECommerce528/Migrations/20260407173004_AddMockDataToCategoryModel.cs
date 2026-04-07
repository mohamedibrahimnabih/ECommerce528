using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce528.Migrations
{
    /// <inheritdoc />
    public partial class AddMockDataToCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (Name, Description, Status) values ('Mobiles', 'Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi, in porttitor pede justo eu massa. Donec dapibus. Duis at velit eu est congue elementum. In hac habitasse platea dictumst. Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante. Nulla justo.', 1); insert into Categories (Name, Description, Status) values ('Tablets', 'Praesent blandit lacinia erat. Vestibulum sed magna at nunc commodo placerat. Praesent blandit.', 1); insert into Categories (Name, Description, Status) values ('Laptops', 'In sagittis dui vel nisl. Duis ac nibh. Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus. Suspendisse potenti. In eleifend quam a odio. In hac habitasse platea dictumst.', 1); insert into Categories (Name, Description, Status) values ('PCs', 'Morbi non quam nec dui luctus rutrum. Nulla tellus. In sagittis dui vel nisl. Duis ac nibh. Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus.', 0); insert into Categories (Name, Description, Status) values ('Accessories', 'Vivamus vel nulla eget eros elementum pellentesque. Quisque porta volutpat erat.', 1); insert into Categories (Name, Description, Status) values ('Cameras', 'Sed ante. Vivamus tortor.', 1); ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Categories");
        }
    }
}
