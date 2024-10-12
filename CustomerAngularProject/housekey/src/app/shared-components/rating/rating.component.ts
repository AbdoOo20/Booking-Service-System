import { Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTooltipModule } from "@angular/material/tooltip";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";

@Component({
    selector: "app-rating",
    standalone: true,
    imports: [FlexLayoutModule, MatIconModule, MatTooltipModule],
    templateUrl: "./rating.component.html",
    styleUrls: ["./rating.component.scss"],
})
export class RatingComponent {
    @Input() ratingsValue: number;
    avg: number;
    stars: Array<string>;
    constructor() {}

    ngDoCheck() {
        if (this.ratingsValue && !this.avg) {
            this.calculateAvgValue();
        }
    }

    public calculateAvgValue() {
        this.avg = this.ratingsValue;
        switch (true) {
            case this.avg > 0 && this.avg < 1: {
                this.stars = [
                    "star_half",
                    "star_border",
                    "star_border",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg == 1: {
                this.stars = [
                    "star",
                    "star_border",
                    "star_border",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg > 1 && this.avg < 2: {
                this.stars = [
                    "star",
                    "star_half",
                    "star_border",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg == 2: {
                this.stars = [
                    "star",
                    "star",
                    "star_border",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg > 2 && this.avg < 3: {
                this.stars = [
                    "star",
                    "star",
                    "star_half",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg == 3: {
                this.stars = [
                    "star",
                    "star",
                    "star",
                    "star_border",
                    "star_border",
                ];
                break;
            }
            case this.avg > 3 && this.avg < 4: {
                this.stars = [
                    "star",
                    "star",
                    "star",
                    "star_half",
                    "star_border",
                ];
                break;
            }
            case this.avg == 4: {
                this.stars = ["star", "star", "star", "star", "star_border"];
                break;
            }
            case this.avg > 4 && this.avg < 5: {
                this.stars = ["star", "star", "star", "star", "star_half"];
                break;
            }
            case this.avg >= 5: {
                this.stars = ["star", "star", "star", "star", "star"];
                break;
            }
            default: {
                this.stars = [
                    "star_border",
                    "star_border",
                    "star_border",
                    "star_border",
                    "star_border",
                ];
                break;
            }
        }
    }
}
