import { Component, HostListener } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { log } from "console";
import { filter } from "rxjs/operators";

@Component({
  selector: "app-logo",
  standalone: true,
  templateUrl: "./logo.component.html",
  styleUrls: ["./logo.css"],
})
export class LogoComponent {
  logoSrc: string = "images/logos/logo-new-white.png";
  isHomePage: boolean = false;

  constructor(private router: Router) {}

  ngOnInit() {
    this.checkIfHomePage();
    this.updateLogoBasedOnScroll();
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkIfHomePage(); // Check if the current page is home after navigation
        this.updateLogoBasedOnScroll(); // Update logo based on the new page
      });
  }

  @HostListener("window:scroll", [])
  onWindowScroll() {
    this.updateLogoBasedOnScroll();
  }

  checkIfHomePage() {
    const currentUrl = this.router.url;
    this.isHomePage =
      currentUrl === "/home" ||
      currentUrl === "/" ||
      currentUrl === "/contact" ||
      currentUrl === "/about";
  }

  updateLogoBasedOnScroll() {
    this.checkIfHomePage();
    if (this.isHomePage) {
      if (window.scrollY === 0) {
        this.logoSrc = "images/logos/logo-new-white.png";
      } else if (window.scrollY > 20) {
        this.logoSrc = "images/logos/logo-new-blue.png";
      }
    } else {
      this.logoSrc = "images/logos/logo-new-blue.png";
    }
  }
}
