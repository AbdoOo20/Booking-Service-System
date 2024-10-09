import {
  Component,
  ElementRef,
  Input,
  NgZone,
  OnInit,
  ViewChild,
} from "@angular/core";
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { MatStepper, MatStepperModule } from "@angular/material/stepper";
import { AppService, Data } from "@services/app.service";
import { DomHandlerService } from "@services/dom-handler.service";
import { InputFileModule } from "../../theme/components/input-file/input-file.module";
import { MatIconModule } from "@angular/material/icon";
import { GoogleMapsModule } from "@angular/google-maps";
import { PipesModule } from "../../theme/pipes/pipes.module";
import { MatButtonModule } from "@angular/material/button";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { BookingService } from "@services/booking.service";
import { Service } from "../../common/interfaces/service";
import { MatNativeDateModule } from "@angular/material/core";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { CommonModule } from "@angular/common";
import { PayPalService } from "@services/pay-pal.service";
import { DataService } from "@services/data.service";

@Component({
  selector: "app-submit-property",
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FlexLayoutModule,
    MatStepperModule,
    MatInputModule,
    MatSelectModule,
    InputFileModule,
    MatIconModule,
    GoogleMapsModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatCheckboxModule,
    PipesModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CommonModule,
  ],
  templateUrl: "./submit-property.component.html",
})
export class SubmitPropertyComponent implements OnInit {
  @ViewChild("horizontalStepper") horizontalStepper: MatStepper;
  @ViewChild("addressAutocomplete") addressAutocomplete: ElementRef;
  public submitForm: FormGroup;
  public features: any[] = [];
  public propertyTypes: any[] = [];
  public propertyStatuses: any[] = [];
  public cities: any[] = [];
  public neighborhoods: any[] = [];
  public streets: any[] = [];
  public lat: number = 40.678178;
  public lng: number = -73.944158;
  center: google.maps.LatLngLiteral = { lat: 40.678178, lng: -73.944158 };
  zoom: number = 12;
  markerPositions: google.maps.LatLngLiteral[] = [
    { lat: 40.678178, lng: -73.944158 },
  ];
  markerOptions: google.maps.MarkerOptions = { draggable: false };
  mapOptions: google.maps.MapOptions = {
    mapTypeControl: true,
    fullscreenControl: true,
  };
  paymentForm: FormGroup;
  constructor(
    public appService: AppService,
    private fb: FormBuilder,
    private ngZone: NgZone,
    private domHandlerService: DomHandlerService,
    public bookService: BookingService,
    private PayPal: PayPalService,
    private dataService: DataService
  ) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 3);
    this.paymentForm = this.fb.group({
      total: [0] // Initialize total with a default value (can be dynamically set later)
    });
  }

  ///////////////////////////////////////////////////////////////////////
  serviceID: number = 1;
  customerID: string = "221d4d90-d8f5-423f-920a-196a0a8c12d8";
  service: Service;
  minDate: Date;
  maxDate: Date;
  startTime: string = "10:00 AM";
  endTime: string = "10:00 PM";
  timeOptions: string[] = [];
  endTimeOptions: string[] = [];
  timeBooked: string[] = [];
  startHour: number;
  endHour: number;
  services: Service[] = [];
  date: string;
  //Payment
  minValue: number;
  maxValue: number;
  total: number;

  // Booking Data
  @Input() BookingData: any;

  ngOnInit() {
    this.features = this.appService.getFeatures();
    this.propertyTypes = this.appService.getPropertyTypes();
    this.propertyStatuses = this.appService.getPropertyStatuses();
    this.cities = this.appService.getCities();
    this.neighborhoods = this.appService.getNeighborhoods();
    this.streets = this.appService.getStreets();
    this.minDate = new Date();
    const currentDate = new Date();
    this.maxDate = new Date(currentDate.setMonth(currentDate.getMonth() + 3));

    this.submitForm = this.fb.group({
      booking: this.fb.group({
        service: [""],
        //desc: [this.service.details],
        //priceDollar: null,
        priceEuro: [""],
        eventDate: ["", Validators.required],
        startTime: ["", Validators.required],
        endTime: ["", Validators.required],
        //propertyType: [null, Validators.required],
        //propertyStatus: null,
        //gallery: null,
      }),
      paymentForm: this.fb.group({
        total: [{ value: 0, disabled: true }, Validators.required], // Total should be disabled for direct input
        minValue: [0],
        maxValue: [0]
      }),
    });
    //set all data here Please Abdo
    this.dataService.setData({
      bookId: 0,
      eventDate: new Date().toISOString(),
      startTime: "string",
      endTime: "string",
      status: "",
      quantity: 1,
      price: 1,
      cashOrCashByHandOrInstallment: "",
      bookDate: new Date().toISOString(),
      type: "Service",
      customerId: "Static ID Here Now",
      serviceId: 0, 
      paymentIncomeId: 0 // need to featch PaymentsIncoms But in The feature, now just PayPal
    })
    this.bookService.getService(this.serviceID).subscribe({
      next: (data) => {
        this.service = data as Service;

        // Set minValue and maxValue from service data
        this.submitForm.patchValue({
          booking: {
            service: this.service.name,
            priceEuro: this.service.priceForTheCurrentDay?.toString(),
          },
          paymentForm: {
            maxValue: this.service.priceForTheCurrentDay?.toString(), // Max value set to Price Euro
            minValue: this.service.initialPayment?.toString(), // Min value set to Initial Payment
          },
        });

        // Optionally convert string to number if needed
        this.minValue = parseFloat(this.submitForm.get('paymentForm.minValue')?.value);
        this.maxValue = parseFloat(this.submitForm.get('paymentForm.maxValue')?.value);

        // Calculate total as the product of initialPayment and priceEuro
        this.calculateTotal();

        this.initializeTimeOptions();
      },
      error: (error) => {
        alert("Error Fetching Service: " + error);
      },
    });

    this.submitForm
      .get("booking.startTime")
      ?.valueChanges.subscribe((startTime) => {
        this.updateEndTimeOptions(startTime);
      });

    this.submitForm.get('paymentForm.minValue')?.valueChanges.subscribe(() => {
      this.calculateTotal();
    });
  }

  // Method to calculate total
  calculateTotal() {
    const initialPayment = parseFloat(this.submitForm.get('paymentForm.minValue')?.value) || 0;
    const priceEuro = parseFloat(this.submitForm.get('booking.priceEuro')?.value) || 0;
    const total = initialPayment * priceEuro;

    // Update the total in the form
    this.submitForm.get('paymentForm.total')?.setValue(total, { emitEvent: false });
  }

  addPayment() {
    // Get total value from the form
    const total = this.paymentForm.get('total')?.value;

    // Ensure minValue and maxValue are properly set from the form
    this.minValue = parseFloat(this.submitForm.get('paymentForm.minValue')?.value);
    this.maxValue = parseFloat(this.submitForm.get('paymentForm.maxValue')?.value);

    // Check if the total is between minValue and maxValue
    if (total < this.minValue || total > this.maxValue || total <= 0) {
      console.warn('Total must be between minimum and maximum values:', total);
      return; // Exit the method if the total is not in the valid range
    }

    const paymentData = {
      total: total,
      currency: "USD",
      description: "New Transaction",
      returnUrl: "http://localhost:4200/confirmation",
      cancelUrl: "http://localhost:4200/submit-property",
    };

    // Call the addPayment method
    this.PayPal.addPayment(paymentData).subscribe({
      next: (response) => {
        window.location.href = response.approvalUrl;
      },
      error: (error) => {
        console.error('Payment Error:', error);
      }
    });
  }

  private initializeTimeOptions() {
    this.timeOptions = [];
    this.startHour = parseInt(this.service.startTime.split(":")[0], 10); // Start from 9 AM
    this.endHour = parseInt(this.service.endTime.split(":")[0], 10); // End at 6 PM
    for (let hour = this.startHour; hour <= this.endHour; hour++) {
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour; // Convert to 12-hour format
      if (!this.timeBooked.includes(`${displayHour} ${amPm}`)) {
        this.timeOptions.push(`${displayHour} ${amPm}`);
      }
    }
  }

  private updateEndTimeOptions(startTime: string) {
    // Reset end time options
    this.endTimeOptions = [];

    if (!startTime) {
      return; // No start time selected
    }

    // Get the hour from the start time
    const [hourStr, period] = startTime.split(" "); // Split time into hour and period (AM/PM)
    let startHour = parseInt(hourStr); // Get hour as number

    // Convert start hour to 24-hour format for comparison
    if (period === "PM" && startHour !== 12) {
      startHour += 12;
    } else if (period === "AM" && startHour === 12) {
      startHour = 0; // Handle midnight case
    }

    // Create end time options starting from the next hour after start time
    for (let hour = startHour + 1; hour <= this.endHour; hour++) {
      // Up to 6 PM
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour; // Convert to 12-hour format
      if (!this.timeBooked.includes(`${displayHour} ${amPm}`)) {
        this.endTimeOptions.push(`${displayHour} ${amPm}`);
      }
    }
  }

  onTimeSelected(selectedDate: Date) {
    this.timeBooked = [];
    this.services = [];
    this.date = "";
    this.submitForm.get("booking.startTime")?.setValue("");
    this.submitForm.get("booking.endTime")?.setValue("");
    const month = (selectedDate.getMonth() + 1).toString().padStart(2, "0");
    const day = selectedDate.getDate().toString().padStart(2, "0");
    const year = selectedDate.getFullYear().toString();
    this.date = `${month}-${day}-${year}`;
    this.bookService
      .getBooingTimeForService(this.serviceID, this.date)
      .subscribe({
        next: (data) => {
          this.services = data as Service[];
          for (const service of this.services) {
            const startTime = parseInt(service.startTime.split(":")[0], 10);
            const endTime = parseInt(service.endTime.split(":")[0], 10);
            for (let hour = startTime; hour <= endTime; hour++) {
              const amPm = hour >= 12 ? "PM" : "AM";
              const displayHour = hour > 12 ? hour - 12 : hour;
              this.timeBooked.push(`${displayHour} ${amPm}`);
            }
          }
          this.initializeTimeOptions();
        },
        error: (error) => {
          alert("Error Fetching Services: " + error);
        },
      });
  }

  public onSelectionChange(e: any) {
    if (e.selectedIndex == 2) {
      this.horizontalStepper._steps.forEach((step) => (step.editable = false));
      console.log(this.submitForm.value);
    }
  }
  public reset() {
    this.horizontalStepper.reset();

    const videos = <FormArray>this.submitForm.controls.media.get("videos");
    while (videos.length > 1) {
      videos.removeAt(0);
    }
    const plans = <FormArray>this.submitForm.controls.media.get("plans");
    while (plans.length > 1) {
      plans.removeAt(0);
    }
    const additionalFeatures = <FormArray>(
      this.submitForm.controls.media.get("additionalFeatures")
    );
    while (additionalFeatures.length > 1) {
      additionalFeatures.removeAt(0);
    }

    this.submitForm.reset({
      additional: {
        features: this.features,
      },
      media: {
        featured: false,
      },
    });
  }

  // -------------------- Address ---------------------------
  public onSelectCity() {
    this.submitForm.controls.address
      .get("neighborhood")!
      .setValue(null, { emitEvent: false });
    this.submitForm.controls.address
      .get("street")!
      .setValue(null, { emitEvent: false });
  }
  public onSelectNeighborhood() {
    this.submitForm.controls.address
      .get("street")!
      .setValue(null, { emitEvent: false });
  }

  private setCurrentPosition() {
    if (this.domHandlerService.isBrowser) {
      if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition((position) => {
          this.lat = position.coords.latitude;
          this.lng = position.coords.longitude;
          this.center = {
            lat: this.lat,
            lng: this.lng,
          };
        });
      }
    }
  }

  onMapReady() {
    setTimeout(() => {
      this.placesAutocomplete();
    });
  }

  private placesAutocomplete() {
    let autocomplete = new google.maps.places.Autocomplete(
      this.addressAutocomplete.nativeElement,
      {
        types: ["address"],
      }
    );
    autocomplete.addListener("place_changed", () => {
      this.ngZone.run(() => {
        let place: google.maps.places.PlaceResult = autocomplete.getPlace();
        if (place.geometry === undefined || place.geometry === null) {
          return;
        }
        this.lat = place.geometry.location!.lat();
        this.lng = place.geometry.location!.lng();
        this.center = {
          lat: this.lat,
          lng: this.lng,
        };
        this.getAddress();
      });
    });
  }

  // public getAddress(){
  //   let geocoder = new google.maps.Geocoder();
  //   let latlng = new google.maps.LatLng(this.lat, this.lng);
  //   geocoder.geocode({'location': latlng}, (results, status) => {
  //     if(status === google.maps.GeocoderStatus.OK) {
  //       console.log(results);
  //       let address = results[0].formatted_address;
  //       this.submitForm.controls.address.get('location').setValue(address);
  //       this.setAddresses(results[0]);
  //     }
  //   });
  // }
  public getAddress() {
    this.appService.getAddress(this.lat, this.lng).subscribe((response) => {
      console.log(response);
      if (response["results"].length) {
        if (response["results"][0]) {
          let address = response["results"][0].formatted_address;
          this.submitForm.controls.address.get("location")!.setValue(address);
          //this.setAddresses(response["results"][0]);
        }
      }
    });
  }
  public onMapClick(e: any) {
    this.lat = e.latLng.lat();
    this.lng = e.latLng.lng();
    this.getAddress();
  }

  // public setAddresses(result: any) {
  //   this.submitForm.controls.address.get("city")!.setValue(null);
  //   this.submitForm.controls.address.get("zipCode")!.setValue(null);
  //   this.submitForm.controls.address.get("street")!.setValue(null);

  //   var newCity, newStreet, newNeighborhood;

  //   result.address_components.forEach((item) => {
  //     if (item.types.indexOf("locality") > -1) {
  //       if (this.cities.filter((city) => city.name == item.long_name)[0]) {
  //         newCity = this.cities.filter(
  //           (city) => city.name == item.long_name
  //         )[0];
  //       } else {
  //         newCity = { id: this.cities.length + 1, name: item.long_name };
  //         this.cities.push(newCity);
  //       }
  //       this.submitForm.controls.address.get("city")!.setValue(newCity);
  //     }
  //     if (item.types.indexOf("postal_code") > -1) {
  //       this.submitForm.controls.address
  //         .get("zipCode")!
  //         .setValue(item.long_name);
  //     }
  //   });

  //   if (!newCity) {
  //     result.address_components.forEach((item) => {
  //       if (item.types.indexOf("administrative_area_level_1") > -1) {
  //         if (this.cities.filter((city) => city.name == item.long_name)[0]) {
  //           newCity = this.cities.filter(
  //             (city) => city.name == item.long_name
  //           )[0];
  //         } else {
  //           newCity = {
  //             id: this.cities.length + 1,
  //             name: item.long_name,
  //           };
  //           this.cities.push(newCity);
  //         }
  //         this.submitForm.controls.address.get("city")!.setValue(newCity);
  //       }
  //     });
  //   }

  //   if (newCity) {
  //     result.address_components.forEach((item) => {
  //       if (item.types.indexOf("neighborhood") > -1) {
  //         let neighborhood = this.neighborhoods.filter(
  //           (n) => n.name == item.long_name && n.cityId == newCity.id
  //         )[0];
  //         if (neighborhood) {
  //           newNeighborhood = neighborhood;
  //         } else {
  //           newNeighborhood = {
  //             id: this.neighborhoods.length + 1,
  //             name: item.long_name,
  //             cityId: newCity.id,
  //           };
  //           this.neighborhoods.push(newNeighborhood);
  //         }
  //         this.neighborhoods = [...this.neighborhoods];
  //         this.submitForm.controls.address
  //           .get("neighborhood")!
  //           .setValue([newNeighborhood]);
  //       }
  //     });
  //   }

  //   if (newCity) {
  //     result.address_components.forEach((item) => {
  //       if (item.types.indexOf("route") > -1) {
  //         if (
  //           this.streets.filter(
  //             (street) =>
  //               street.name == item.long_name && street.cityId == newCity.id
  //           )[0]
  //         ) {
  //           newStreet = this.streets.filter(
  //             (street) =>
  //               street.name == item.long_name && street.cityId == newCity.id
  //           )[0];
  //         } else {
  //           newStreet = {
  //             id: this.streets.length + 1,
  //             name: item.long_name,
  //             cityId: newCity.id,
  //             neighborhoodId: newNeighborhood ? newNeighborhood.id : null,
  //           };
  //           this.streets.push(newStreet);
  //         }
  //         this.streets = [...this.streets];
  //         this.submitForm.controls.address.get("street")!.setValue([newStreet]);
  //       }
  //     });
  //   }
  // }

  // -------------------- Additional ---------------------------
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

  // -------------------- Media ---------------------------
  public createVideo(): FormGroup {
    return this.fb.group({
      id: null,
      name: null,
      link: null,
    });
  }
  public addVideo(): void {
    const videos = this.submitForm.controls.media.get("videos") as FormArray;
    videos.push(this.createVideo());
  }
  public deleteVideo(index) {
    const videos = this.submitForm.controls.media.get("videos") as FormArray;
    videos.removeAt(index);
  }

  public createPlan(): FormGroup {
    return this.fb.group({
      id: null,
      name: null,
      desc: null,
      area: null,
      rooms: null,
      baths: null,
      image: null,
    });
  }
  public addPlan(): void {
    const plans = this.submitForm.controls.media.get("plans") as FormArray;
    plans.push(this.createPlan());
  }
  public deletePlan(index) {
    const plans = this.submitForm.controls.media.get("plans") as FormArray;
    plans.removeAt(index);
  }

  public createFeature(): FormGroup {
    return this.fb.group({
      id: null,
      name: null,
      value: null,
    });
  }
  public addFeature(): void {
    const features = this.submitForm.controls.media.get(
      "additionalFeatures"
    ) as FormArray;
    features.push(this.createFeature());
  }
  public deleteFeature(index) {
    const features = this.submitForm.controls.media.get(
      "additionalFeatures"
    ) as FormArray;
    features.removeAt(index);
  }

  get featuresForm() {
    return (this.submitForm.get("additional") as FormGroup).controls
      .features as FormArray;
  }
}
