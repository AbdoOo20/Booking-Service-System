import { NgClass } from "@angular/common";
import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import {
    FormBuilder,
    FormGroup,
    NgModel,
    ReactiveFormsModule,
} from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import {
    FloatLabelType,
    MatFormFieldAppearance,
} from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatSelectChange, MatSelectModule } from "@angular/material/select";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { TranslateModule } from "@ngx-translate/core";
import { AppService } from "@services/app.service";
import { PipesModule } from "../../theme/pipes/pipes.module";
import { CategoriesService } from "@services/categories.service";
import { Category } from "../../common/interfaces/category";
import { log } from "console";
import { HomeComponent } from "../../pages/home/home.component";
import { ServicesService } from "@services/services.service";
import { Service } from "../../common/interfaces/service";

@Component({
    selector: "app-properties-search",
    standalone: true,
    imports: [
        ReactiveFormsModule,
        FlexLayoutModule,
        NgClass,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatIconModule,
        MatCheckboxModule,
        TranslateModule,
        PipesModule,
    ],
    providers: [CategoriesService, ServicesService],
    templateUrl: "./properties-search.component.html",
})
export class PropertiesSearchComponent implements OnInit {
    @Input() category: Category;
    @Input() variant: number = 1;
    @Input() vertical: boolean = false;
    @Input() searchOnBtnClick: boolean = false;
    @Input() removedSearchField: string;
    @Output() onSearchChange: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSearchClick: EventEmitter<any> = new EventEmitter<any>();
    public showMore: boolean = false;
    public form: FormGroup;
    public propertyTypes: any[] = [];
    public propertyStatuses: any[] = [];
    public cities: any[] = [];
    public neighborhoods: any[] = [];
    public streets: any[] = [];
    public features: any[] = [];
    // My Property
    public CategoryNames: Category[];
    public ServicesItems: Service[];
    public catChangeName = "";
    public locChangeName = "";
    public LocationNames: any[] = [];
    public fromPrice = 0;
    public toPrice = Number.MAX_VALUE;
    public ValueFromInput: number;
    public ValueToInput: number;

    constructor(
        public appService: AppService,
        public fb: FormBuilder,
        public CatServ: CategoriesService,
        private homeCmp: HomeComponent,
        public myServ: ServicesService
    ) {}

    ngOnInit() {
        if (this.vertical) {
            this.showMore = true;
        }
        this.propertyTypes = this.appService.getPropertyTypes();
        this.propertyStatuses = this.appService.getPropertyStatuses();
        this.cities = this.appService.getCities();
        this.neighborhoods = this.appService.getNeighborhoods();
        this.streets = this.appService.getStreets();
        this.features = this.appService.getFeatures();

        this.form = this.fb.group({
            propertyType: null,
            categoryType: null,
            propertyStatus: null,
            price: this.fb.group({
                from: 0,
                to: 0,
            }),
            city: null,
            zipCode: null,
            neighborhood: null,
            street: null,
            bedrooms: this.fb.group({
                from: null,
                to: null,
            }),
            bathrooms: this.fb.group({
                from: null,
                to: null,
            }),
            garages: this.fb.group({
                from: null,
                to: null,
            }),
            area: this.fb.group({
                from: null,
                to: null,
            }),
            yearBuilt: this.fb.group({
                from: null,
                to: null,
            }),
            features: this.buildFeatures(),
        });

        this.onSearchChange.emit(this.form);
        //My Code
        this.getCategories();
        this.getServiceCategories(this.catChangeName);
        this.getServiceLocation(this.locChangeName);
        this.getLocations();
        // this.getServicePrice(this.fromPrice);
        this.getPrice();
    }

    public buildFeatures() {
        const arr = this.features.map((feature) => {
            return this.fb.group({
                id: feature.id,
                name: feature.name,
                selected: feature.selected,
            });
        });
        return this.fb.array(arr);
    }

    ngOnChanges() {
        if (this.removedSearchField) {
            if (this.removedSearchField.indexOf(".") > -1) {
                let arr = this.removedSearchField.split(".");
                this.form.controls[arr[0]]["controls"][arr[1]].reset();
            } else if (this.removedSearchField.indexOf(",") > -1) {
                let arr = this.removedSearchField.split(",");
                this.form.controls[arr[0]]["controls"][arr[1]]["controls"][
                    "selected"
                ].setValue(false);
            } else {
                this.form.controls[this.removedSearchField].reset();
            }
        }
    }

    public reset() {
        this.homeCmp.getServices();
        this.form.reset({
            propertyType: null,
            categoryType: null,
            propertyStatus: null,
            price: {
                from: 0,
                to: 0,
            },
            city: null,
            zipCode: null,
            neighborhood: null,
            street: null,
            bedrooms: {
                from: null,
                to: null,
            },
            bathrooms: {
                from: null,
                to: null,
            },
            garages: {
                from: null,
                to: null,
            },
            area: {
                from: null,
                to: null,
            },
            yearBuilt: {
                from: null,
                to: null,
            },
            features: this.features,
        });
    }

    public search() {
        this.onSearchClick.emit();
    }

    public onSelectCity() {
        this.form.controls["neighborhood"].setValue(null, { emitEvent: false });
        this.form.controls["street"].setValue(null, { emitEvent: false });
    }
    public onSelectNeighborhood() {
        this.form.controls["street"].setValue(null, { emitEvent: false });
    }

    public getAppearance(): MatFormFieldAppearance {
        return this.variant != 3 ? "outline" : "fill";
    }
    public getFloatLabel(): FloatLabelType {
        return this.variant == 1 ? "always" : "auto";
    }
    // My Functions
    filterCats(e: any) {
        // let value = e.target.value;
        this.homeCmp.getServiceCategories(e.name);
        console.log(e.name);
    }
    filterLocs(e: any) {
        // let value = e.target.value;
        this.homeCmp.getServiceLocation(e.name_en);
        console.log(e.name_en);
    }
    getCategories() {
        this.CatServ.GetAllCategories().subscribe({
            next: (data) => {
                this.CategoryNames = data as Category[];
                console.log(data);
            },
            error: (err) => {
                console.log(err);
            },
        });
    }

    getLocations(): void {
        this.myServ.getLocations().subscribe({
            next: (data) => {
                this.LocationNames = data;
                console.log(data);
            },
            error: (err) => {
                console.error("Error fetching regions data:", err);
            },
        });
    }
    public getServiceCategories(catName) {
        this.myServ.GetAllServicesByCatName(catName).subscribe({
            next: (data) => {
                this.ServicesItems = data;
                console.log(data);
            },
            error: (err) => {
                console.log(err);
            },
        });
    }
    public getServiceLocation(locName) {
        this.myServ.GetAllServicesByCatName(locName).subscribe({
            next: (data) => {
                this.ServicesItems = data;
                console.log(data);
            },
            error: (err) => {
                console.log(err);
            },
        });
    }
    // getInputPriceValue() {
    //     this.form.get("price.from")?.valueChanges.subscribe((value) => {
    //         this.homeCmp.getServicePrice(value, Number.MIN_VALUE);
    //         console.log(value);
    //     });
    //     this.form.get("price.to")?.valueChanges.subscribe((value) => {
    //         this.homeCmp.getServicePrice(0, value);
    //         console.log(value);
    //     });
    // }

    getPrice() {
        this.form.get("price.from")?.valueChanges.subscribe((value) => {
            this.ValueFromInput = value;
            console.log(this.ValueFromInput + " " + this.ValueToInput);
            this.homeCmp.getServicePrice(
                this.ValueFromInput,
                this.ValueToInput
            );
        });
        this.form.get("price.to")?.valueChanges.subscribe((value) => {
            this.ValueToInput = value;
            console.log(this.ValueFromInput + " " + this.ValueToInput);
            this.homeCmp.getServicePrice(
                this.ValueFromInput,
                this.ValueToInput
            );
        });
    }
    // public getServicePrice(from: number) {
    //     this.myServ.GetAllServicesPrice(from).subscribe({
    //         next: (data) => {
    //             this.ServicesItems = data;
    //             console.log(data);
    //         },
    //         error: (err) => {
    //             console.log(err);
    //         },
    //     });
    // }
}
