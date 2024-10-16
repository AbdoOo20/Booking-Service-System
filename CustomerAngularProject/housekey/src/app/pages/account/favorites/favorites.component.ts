import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { Component, OnInit, ViewChild } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { WishlistService } from "@services/wishlist.service";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatTableModule } from "@angular/material/table";
import { MatIconModule } from "@angular/material/icon";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatSortModule } from "@angular/material/sort";
import { MatTable } from "@angular/material/table";
import { Property } from "@models/app.models";
import { DecodingTokenService } from "@services/decoding-token.service";

@Component({
  selector: "app-favorites",
  standalone: true,
  templateUrl: "./favorites.component.html",
  imports: [
    CommonModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatIconModule,
    MatSortModule,
  ],
  providers: [DecodingTokenService],
})
export class FavoritesComponent implements OnInit {
  displayedColumns: string[] = [
    "name",
    "location",
    "price",
    "image",
    "actions",
  ];
  //dataSource: MatTableDataSource<any>;
  dataSource: MatTableDataSource<Property> = new MatTableDataSource();
  @ViewChild(MatTable) table: MatTable<any>;
  customerId: string;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private wishlistService: WishlistService,
    public DecodingCustomerID: DecodingTokenService
  ) {}

  ngOnInit() {
    this.customerId = this.DecodingCustomerID.getUserIdFromToken();
    this.loadWishlist(this.customerId); // استخدم customerId الفعلي هنا
  }

  loadWishlist(customerId: string) {
    this.wishlistService.getWishlistServices(customerId).subscribe(
      (data) => {
        let properties: Property[] = JSON.parse(data);
        this.dataSource = new MatTableDataSource<Property>(properties);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      (error) => {
        console.error("Error loading wishlist", error);
      }
    );
  }

  public remove(property: Property) {
    const index: number = this.dataSource.data.indexOf(property);

    if (index !== -1) {
      this.dataSource.data.splice(index, 1);
      this.dataSource = new MatTableDataSource<Property>(this.dataSource.data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      // استدعاء خدمة حذف العنصر

      this.wishlistService
        .deleteServiceFromWishlist(this.customerId, property.id)
        .subscribe(
          (response) => {
            // إزالة العنصر من المصفوفة
            //this.dataSource.data.splice(index, 1);

            // تحديث الـ MatTableDataSource
            // this.dataSource = new MatTableDataSource<Property>(this.dataSource.data);
            // this.dataSource.paginator = this.paginator;
            //this.dataSource.sort = this.sort;

            // تحديث الجدول بدون إعادة تحميل الصفحة
            this.table.renderRows();
          },
          (error) => {
            console.error("Error deleting service:", error);
          }
        );
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatIconModule,
    MatSortModule,
    FavoritesComponent, // استيراد المكون هنا
  ],
})
export class FavoritesModule {}
