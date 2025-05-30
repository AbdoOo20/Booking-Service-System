import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { MatChipsModule } from "@angular/material/chips";
import { MatIconModule } from "@angular/material/icon";
import { HomeComponent } from "../../pages/home/home.component";

@Component({
    selector: "app-properties-search-results-filters",
    standalone: true,
    imports: [MatChipsModule, MatIconModule],
    templateUrl: "./properties-search-results-filters.component.html",
})
export class PropertiesSearchResultsFiltersComponent implements OnInit {
    @Input() searchFields: any;
    @Output() onRemoveSearchField: EventEmitter<any> = new EventEmitter<any>();
    constructor(private homeCmp: HomeComponent) {}

    ngOnInit() {}

    public remove(field) {
        // this.homeCmp.getServ(
        //     this.searchFields.propertyType.name,
        //     this.searchFields.city.name_en,
        //     this.searchFields.price.from,
        //     this.searchFields.price.to
        // );
        // console.log(
        //     this.homeCmp.getServ(
        //         this.searchFields.propertyType.name,
        //         this.searchFields.city.name_en,
        //         this.searchFields.price.from,
        //         this.searchFields.price.to
        //     )
        // );
        this.homeCmp.getServices();

        this.onRemoveSearchField.emit(field);
    }
}
