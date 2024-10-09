import {
    Component,
    OnInit,
    Input,
    ViewChild,
    SimpleChange,
    ChangeDetectorRef,
} from "@angular/core";
import { Property } from "@models/app.models";
import { SwiperDirective } from "../../theme/components/swiper/swiper.directive";
import {
    SwiperConfigInterface,
    SwiperModule,
    SwiperPaginationInterface,
} from "../../theme/components/swiper/swiper.module";
import { Settings, SettingsService } from "@services/settings.service";
import { AppService } from "@services/app.service";
import { CompareOverviewComponent } from "@shared-components/compare-overview/compare-overview.component";
import { MatCardModule } from "@angular/material/card";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { RouterModule } from "@angular/router";
import { CurrencyPipe, DatePipe } from "@angular/common";
import { RatingComponent } from "@shared-components/rating/rating.component";
import { ServicesService } from "@services/services.service";
import { Service } from "../../common/interfaces/service";

@Component({
    selector: "app-recommendation-booking",
    standalone: true,
    imports: [
        RouterModule,
        FlexLayoutModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        SwiperModule,
        CurrencyPipe,
        RatingComponent,
        DatePipe,
    ],
    providers: [ServicesService],
    templateUrl: "./recommendation-booking.component.html",
    styleUrl: "./recommendation-booking.component.scss",
})
export class RecommendationBookingComponent implements OnInit {
    @Input() service: Service;
    @Input() property: Property;
    @Input() viewType: string = "grid";
    @Input() viewColChanged: number = 0;
    @Input() fullWidthPage: boolean = true;
    public column: number = 4;
    @ViewChild(SwiperDirective) directiveRef: SwiperDirective;
    public config: SwiperConfigInterface = {};
    private pagination: SwiperPaginationInterface = {
        el: ".swiper-pagination",
        clickable: true,
    };
    public settings: Settings;
    constructor(
        public settingsService: SettingsService,
        public appService: AppService,
        public cdr: ChangeDetectorRef,
        public myServ: ServicesService
    ) {
        this.settings = this.settingsService.settings;
    }
    // My Function
    public getImageUrl(imagePath: string): string {
        const baseUrl = "http://localhost:30480";
        return `${baseUrl}${imagePath}`;
    }
    ngOnInit() {}
    public addToCompare() {
        this.appService.addToCompare(
            this.property,
            CompareOverviewComponent,
            this.settings.rtl ? "rtl" : "ltr"
        );
    }

    public onCompare() {
        return this.appService.Data.compareList.filter(
            (item) => item.id == this.property.id
        )[0];
    }

    public addToFavorites() {
        this.appService.addToFavorites(
            this.property,
            this.settings.rtl ? "rtl" : "ltr"
        );
    }

    public onFavorites() {
        return this.appService.Data.favorites.filter(
            (item) => item.id == this.property.id
        )[0];
    }
}
