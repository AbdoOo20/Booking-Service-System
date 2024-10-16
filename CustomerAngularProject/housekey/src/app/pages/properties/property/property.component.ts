import {
  Component,
  HostListener,
  OnInit,
  Output,
  output,
  QueryList,
  ViewChild,
  ViewChildren,
} from "@angular/core";
import {
  SwiperConfigInterface,
  SwiperDirective,
  SwiperModule,
} from "../../../theme/components/swiper/swiper.module";
import { Property } from "@models/app.models";
import { Settings, SettingsService } from "@services/settings.service";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { AppService } from "@services/app.service";
import { ActivatedRoute, RouterModule } from "@angular/router";
import { EmbedVideoService } from "@services/embed-video.service";
import { DomHandlerService } from "@services/dom-handler.service";
import { emailValidator } from "../../../theme/utils/app-validators";
import { CompareOverviewComponent } from "@shared-components/compare-overview/compare-overview.component";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatIconModule } from "@angular/material/icon";
import { NgScrollbarModule } from "ngx-scrollbar";
import { MatCardModule } from "@angular/material/card";
import { RatingComponent } from "@shared-components/rating/rating.component";
import { MatDividerModule } from "@angular/material/divider";
import { MatInputModule } from "@angular/material/input";
import { PropertyItemComponent } from "@shared-components/property-item/property-item.component";
import { CurrencyPipe, DatePipe, NgClass } from "@angular/common";
import { GoogleMapsModule } from "@angular/google-maps";
import { MatExpansionModule } from "@angular/material/expansion";
import { CommentsComponent } from "@shared-components/comments/comments.component";
import { PropertiesCarouselComponent } from "@shared-components/properties-carousel/properties-carousel.component";
import { GetInTouchComponent } from "@shared-components/get-in-touch/get-in-touch.component";
import { MatButtonModule } from "@angular/material/button";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { ServiceDetails } from "../../../common/interfaces/ServiceDetails";
import { ServicesService } from "@services/services.service";
import { Service } from "../../../common/interfaces/service";
import { provider } from "../../../common/interfaces/provider";
import { AllBookingsService } from "@services/all-bookings.service";
import { ReviewServiceService } from "@services/review-service.service";
import { ReviewFormComponent } from "../ReviewForm/review-form.component";
import { ReviewsComponent } from "../Reviews/reviews.component";
import { DecodingTokenService } from "@services/decoding-token.service";
import { Observable, throwIfEmpty } from "rxjs";
import { WishlistService } from "@services/wishlist.service";


@Component({
  selector: "app-property",
  standalone: true,
  imports: [
    RouterModule,
    ReactiveFormsModule,
    SwiperModule,
    MatSidenavModule,
    MatIconModule,
    NgScrollbarModule,
    MatCardModule,
    RatingComponent,
    MatDividerModule,
    MatInputModule,
    PropertyItemComponent,
    CurrencyPipe,
    NgClass,
    GoogleMapsModule,
    MatExpansionModule,
    DatePipe,
    CommentsComponent,
    PropertiesCarouselComponent,
    GetInTouchComponent,
    MatButtonModule,
    FlexLayoutModule,
    ReviewFormComponent,
    ReviewsComponent,
  ],
  templateUrl: "./property.component.html",
  styleUrl: "./property.component.scss",

  providers: [
    EmbedVideoService,
    AllBookingsService,
    ReviewServiceService,
    DecodingTokenService,WishlistService
  ],
})
export class PropertyComponent implements OnInit {
  @ViewChild("sidenav") sidenav: any;
  @ViewChildren(SwiperDirective) swipers: QueryList<SwiperDirective>;
  public sidenavOpen: boolean = true;
  public config: SwiperConfigInterface = {};
  public config2: SwiperConfigInterface = {};
  private sub: any;
  private subService: any;
  public property: Property;
  public service: any;
  public settings: Settings;
  public embedVideo: any;
  public relatedProperties: Property[];
  public relatedServices: Service[];
  public featuredProperties: Property[];
  public agent: any;
  public baseUrlImage = "http://localhost:30480";
  public mortgageForm: FormGroup;
  public monthlyPayment: any;
  public contactForm: FormGroup;
  public provider: provider;
  public allBookings: any[];
  public rev: any[] = [];
  public Services_WishList: any;
  public customerId: string;
  public servicesIDs: number[] = [];
  @Output() bookId: number[] = [];
  public serviceID:number;
  mapOptions: google.maps.MapOptions = {
    mapTypeControl: true,
    fullscreenControl: true,
  };
  lat: number = 0;
  lng: number = 0;
  token = localStorage.getItem("token");
    

  constructor(
    public settingsService: SettingsService,
    public appService: AppService,
    private myServ: ServicesService,
    private activatedRoute: ActivatedRoute,
    private embedService: EmbedVideoService,
    public fb: FormBuilder,

    private domHandlerService: DomHandlerService,
    public bookservice: AllBookingsService, //bookingService
    public ReviewService: ReviewServiceService, //ReviewService
    public DecodingCustomerID: DecodingTokenService,public wishListService:WishlistService
  ) {
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    // this.sub = this.activatedRoute.params.subscribe((params) => {
    //     this.getPropertyById(params["id"]);
    // });
    // this.customerId=this.DecodingCustomerID.getUserIdFromToken();
    this.activatedRoute.paramMap.subscribe((params)=>{
      this.serviceID=Number(params.get("id"));
      console.log(this.serviceID)})


    this.sub = this.activatedRoute.params.subscribe((params) => {
      this.getSericeById(params["id"]);
    });
    this.subService = this.activatedRoute.params.subscribe((params) => {
      this.getSericeById(params["id"]);
    });
    this.getRelatedProperties();
    // this.getRelatedServices();
    this.getFeaturedProperties();
    this.getAgent(1);
    if (this.domHandlerService.window?.innerWidth < 960) {
      this.sidenavOpen = false;
      if (this.sidenav) {
        this.sidenav.close();
      }
    }
    this.mortgageForm = this.fb.group({
      principalAmount: ["", Validators.required],
      downPayment: ["", Validators.required],
      interestRate: ["", Validators.required],
      period: ["", Validators.required],
    });
    this.contactForm = this.fb.group({
      name: ["", Validators.required],
      email: ["", Validators.compose([Validators.required, emailValidator])],
      phone: ["", Validators.required],
      message: ["", Validators.required],
    });
    this.loadWishlistServices()
    this.checkForReview();
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
    this.subService.unsubscribe();
  }

  @HostListener("window:resize")
  public onWindowResize(): void {
    this.domHandlerService.window?.innerWidth < 960
      ? (this.sidenavOpen = false)
      : (this.sidenavOpen = true);
  }

  public getPropertyById(id: number) {
    this.appService.getPropertyById(id).subscribe((data) => {
      this.property = data;
      this.embedVideo = this.embedService.embed(this.property.videos[1].link);
      // this.lat = +this.property.location.lat;
      // this.lng = +this.property?.location.lng;
      if (this.domHandlerService.isBrowser) {
        this.config.observer = false;
        this.config2.observer = false;
        setTimeout(() => {
          this.config.observer = true;
          this.config2.observer = true;
          this.swipers.forEach((swiper) => {
            if (swiper) {
              swiper.setIndex(0);
            }
          });
        });
      }
    });
  }
  public getSericeById(id) {
    this.myServ.getServiceById(id).subscribe({
      next: (data) => {
        this.service = data;
        //console.log(data.provider.providerId);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  ngAfterViewInit() {
    this.config = {
      observer: true,
      slidesPerView: 1,
      spaceBetween: 0,
      keyboard: true,
      navigation: true,
      pagination: false,
      grabCursor: true,
      loop: false,
      preloadImages: false,
      lazy: true,
      autoplay: {
        delay: 5000,
        disableOnInteraction: false,
      },
    };

    this.config2 = {
      observer: true,
      slidesPerView: 4,
      spaceBetween: 16,
      keyboard: true,
      navigation: false,
      pagination: false,
      grabCursor: true,
      loop: false,
      preloadImages: false,
      lazy: true,
      breakpoints: {
        200: {
          slidesPerView: 2,
        },
        480: {
          slidesPerView: 3,
        },
        600: {
          slidesPerView: 4,
        },
      },
    };
  }

  public onOpenedChange() {
    this.swipers.forEach((swiper) => {
      if (swiper) {
        swiper.update();
      }
    });
  }

  public selectImage(index: number) {
    this.swipers.forEach((swiper) => {
      if (swiper["elementRef"].nativeElement.id == "main-carousel") {
        swiper.setIndex(index);
      }
    });
  }

  public onIndexChange(index: number) {
    this.swipers.forEach((swiper) => {
      let elem = swiper["elementRef"].nativeElement;
      if (elem.id == "small-carousel") {
        swiper.setIndex(index);
        for (let i = 0; i < elem.children[0].children.length; i++) {
          const element = elem.children[0].children[i];
          if (element.classList.contains("thumb-" + index)) {
            element.classList.add("active-thumb");
          } else {
            element.classList.remove("active-thumb");
          }
        }
      }
    });
  }

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
    this.myServ.addToFavoritesInServiceDetails(
      this.service,
      this.settings.rtl ? "rtl" : "ltr",
      this.customerId
    );
  }
  public loadWishlistServices() {
    this.wishListService.getWishlistServices(this.customerId).subscribe({
      next: (data) => {
        // Parse the response only if it's a string
        if (typeof data === 'string') {
          try {
            data = JSON.parse(data);
          } catch (error) {
            console.error('Failed to parse JSON string:', error);
            return;
          }
        }
  
        // Ensure the data is an array and populate service IDs
        if (Array.isArray(data)) {
          this.servicesIDs = data.map(service => service.id);
        } else {
          console.error('Expected an array but got:', typeof data);
        }
      },
      error: (err) => {
        console.error('Error fetching wishlist services:', err);
      }
    });
  }
  
  // Check if the service is already in favorites
  public onFavorites(): boolean {
    return this.servicesIDs.includes(this.service.id);  // Simplified check
  }
  

 

  public getRelatedProperties() {
    this.appService.getRelatedProperties().subscribe((data) => {
      this.relatedProperties = data;
    });
  }
  // public getRelatedServices() {
  //     this.myServ.getRelatedServices(id).subscribe((data) => {
  //         this.relatedServices = data;
  //     });
  // }

  public getFeaturedProperties() {
    this.appService.getFeaturedProperties().subscribe((properties) => {
      this.featuredProperties = properties.slice(0, 3);
    });
  }

  public getAgent(agentId: number = 1) {
    var ids = [1, 2, 3, 4, 5]; //agent ids
    agentId = ids[Math.floor(Math.random() * ids.length)]; //random agent id
    this.agent = this.appService
      .getAgents()
      .filter((agent) => agent.id == agentId)[0];
  }

  public onContactFormSubmit(values: Object) {
    if (this.contactForm.valid) {
      console.log(values);
    }
  }

  public onMortgageFormSubmit(values: Object) {
    if (this.mortgageForm.valid) {
      var principalAmount = values["principalAmount"];
      var down = values["downPayment"];
      var interest = values["interestRate"];
      var term = values["period"];
      this.monthlyPayment = this.calculateMortgage(
        principalAmount,
        down,
        interest / 100 / 12,
        term * 12
      ).toFixed(2);
    }
  }
  public calculateMortgage(
    principalAmount: any,
    downPayment: any,
    interestRate: any,
    period: any
  ) {
    return (
      ((principalAmount - downPayment) * interestRate) /
      (1 - Math.pow(1 + interestRate, -period))
    );
  }

 
  checkForReview() {
    // Get customer ID
    this.customerId = this.DecodingCustomerID.getUserIdFromToken(); // Ensure this is returning a valid ID
  
    // Log before the API call
    console.log('Fetching reviews for customer:', this.customerId, 'and service:', this.serviceID);
  
    // Fetch reviews
    this.ReviewService.getAllReviewsForBookings(this.customerId, this.serviceID).subscribe({
      next: (data) => {
        console.log('Received review data:', data);  // Log the received data
  
        // Clear the rev array before adding new reviews
        this.rev = [];
  
        // Process the bookings and reviews
        data.forEach(booking => {
          if (booking.reviews && booking.reviews.length > 0) {
            // Append the reviews to the rev array
            this.rev.push(...booking.reviews);
          } else {
            // If no reviews, add bookingId for future use
            this.bookId.push(booking.bookId); 
          }
        });
  
        // Log the final reviews array
        console.log('All reviews:', this.rev);
      },
      error: (err) => {
        console.error('Error fetching reviews:', err);  // More detailed error logging
      }
    });
  }
  
  // Handle review submission
  handleReviewSubmission(reviewData: { rating: number; reply: string }) {
    // Add the new review to the rev array
    this.rev.push(reviewData);
    console.log('New review added:', reviewData);
  
    // Recheck reviews after adding a new one
    this.checkForReview();
  }
  
    
    }
    
