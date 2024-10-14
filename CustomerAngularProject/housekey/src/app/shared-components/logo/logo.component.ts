import { Component, HostListener } from "@angular/core";

@Component({
    selector: "app-logo",
    standalone: true,
    templateUrl: "./logo.component.html",
    styleUrls: ["./logo.css"],
})
export class LogoComponent {
    logoSrc: string = "images/logos/logo-new-white.png";
    @HostListener("window:scroll", [])
    onWindowScroll() {
        if (window.scrollY > 0) {
            this.logoSrc = "images/logos/logo-new-black.png";
        } else {
            this.logoSrc = "images/logos/logo-new-white.png";
        }
    }
}
