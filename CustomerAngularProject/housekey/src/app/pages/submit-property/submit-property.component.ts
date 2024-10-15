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
import { AppService } from "@services/app.service";
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
import { CommonModule, Time } from "@angular/common";
import { PayPalService } from "@services/pay-pal.service";
import { DataService } from "@services/data.service";
import { MatDialog } from "@angular/material/dialog";
import { ContractDialogComponent } from "./../../shared-components/contract-dialog/contract-dialog.component";
import { MatDialogModule } from "@angular/material/dialog";
import { AbstractControl, ValidationErrors } from "@angular/forms";
import { Observable, of } from "rxjs";
import { delay } from "rxjs/operators";
import { SharedService } from "@services/shared.service";
import { _SharedService } from "@services/passing-data.service";
import { DecodingTokenService } from "@services/decoding-token.service";
import { ActivatedRoute } from "@angular/router";
import { Router } from "@angular/router";

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
    MatDialogModule,
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

  ///////////////////////////////////////////////////////////////////////
  serviceID: number; //221d4d90-d8f5-423f-920a-196a0a8c12d8
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
  isContractAccepted: boolean = false;
  hasQuantity: boolean = false;
  acceptContract: boolean = false;
  bookedQuantity = 0;
  maxQuantity = 0;
  EndTimeForLastBook: number = 0;

  // //Payment
  public payment: FormGroup;
  public minValue: number;
  public maxValue: number;
  public amount: number;

  // Booking Data
  bookingData: any;
  public eventBookingDate: string;
  public startBookingTime: string;
  public endBookingTime: string;
  CustomerIDFromToken: any;

  constructor(
    public appService: AppService,
    private fb: FormBuilder,
    private ngZone: NgZone,
    private domHandlerService: DomHandlerService,
    public bookService: BookingService,
    private PayPal: PayPalService,
    private dialog: MatDialog,
    private sharedService: SharedService,
    private NewBooking: _SharedService,
    private decodingService: DecodingTokenService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    // this.total = 0;
  }

  asyncQuantityValidator(serviceQuantity: number) {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) {
        return of(null);
      }
      return of(
        control.value > serviceQuantity ? { invalidQuantity: true } : null
      ).pipe(delay(500));
    };
  }

  ngOnInit() {
    if (!localStorage.getItem("token")) {
      const targetPage = "/login";
      this.router.navigate([targetPage]);
    }
    this.activatedRoute.paramMap.subscribe((params) => {
      this.serviceID = Number(params.get("id")); // استقبال الـ id
      console.log(this.serviceID); // طباعة الـ id
    });
    this.features = this.appService.getFeatures();
    this.propertyTypes = this.appService.getPropertyTypes();
    this.propertyStatuses = this.appService.getPropertyStatuses();
    this.cities = this.appService.getCities();
    this.neighborhoods = this.appService.getNeighborhoods();
    this.streets = this.appService.getStreets();
    this.minDate = new Date();
    const currentDate = new Date();
    this.maxDate = new Date(currentDate.setMonth(currentDate.getMonth() + 3));
    this.CustomerIDFromToken = this.decodingService.getUserIdFromToken();

    this.submitForm = this.fb.group({
      booking: this.fb.group({
        service: [""],
        priceEuro: [""],
        eventDate: ["", Validators.required],
        startTime: ["", Validators.required],
        endTime: ["", Validators.required],
        quantity: [""],
      }),
      payment: this.fb.group({
        amount: [0, Validators.required],
        minValue: 0,
        maxValue: 0,
      }),
    });
    const today = new Date();
    this.minDate = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate() + 5
    );
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 3);
    this.bookService.getService(this.serviceID).subscribe({
      next: (data) => {
        this.hasQuantity = false;
        this.service = data as Service;
        this.submitForm.patchValue({
          booking: {
            service: this.service.name,
            priceEuro: this.service.priceForTheCurrentDay?.toString(),
            quantity: null,
          },
        });
        this.submitForm.patchValue({
          payment: {
            minValue: this.service.initialPayment,
            maxValue: this.service.priceForTheCurrentDay,
          },
        });
        this.hasQuantity = this.service.quantity > 0 ? true : false;
        this.calculateTotal();
        this.initializeTimeOptions();

        const quantityControl = this.submitForm.get("booking.quantity");
        if (this.service.quantity > 0) {
          quantityControl?.setAsyncValidators(
            this.asyncQuantityValidator(this.service.quantity)
          );
        } else {
          quantityControl?.clearAsyncValidators(); // Clear async validators if not needed
        }
        quantityControl?.updateValueAndValidity();
      },
      error: (error) => {
        this.hasQuantity = false;
        alert("Error Fetching Service: " + error);
      },
    });
    this.submitForm
      .get("booking.startTime")
      ?.valueChanges.subscribe((startTime) => {
        this.updateEndTimeOptions(startTime);
      });
    this.initializeTimeOptions();
    this.calculateTotal();
  }

  shareData(): void {
    // Convert booking form data
    const selectedDate = this.submitForm.get("booking.eventDate").value;
    const convertedStartTime = this.convertTo24HourFormat(
      this.submitForm.get("booking.startTime").value
    );
    const convertedEndTime = this.convertTo24HourFormat(
      this.submitForm.get("booking.endTime").value
    );
    console.log(selectedDate);

    this.bookingData = {
      bookId: 0,
      eventDate: selectedDate,
      startTime: convertedStartTime,
      endTime: convertedEndTime,
      initialPaymentPercentage: 20,
      status: "Pending",
      quantity: Number(this.submitForm.get("booking.quantity").value),
      price: parseFloat(this.submitForm.get("booking.priceEuro").value),
      cashOrCashByHandOrInstallment: "Cash",
      bookDate: new Date().toISOString(),
      type: "Service",
      customerId: this.CustomerIDFromToken,
      serviceId: this.serviceID,
      paymentIncomeId: 1,
    };

    // Save data in local storage
    localStorage.setItem("bookingData", JSON.stringify(this.bookingData));

    // Set data in SharedService
    this.sharedService.setData(this.bookingData);
    console.log("Booking data shared:", this.bookingData);
    this.NewBooking.setData(this.bookingData);
  }

  convertTo24HourFormat(time: string): string {
    // Parse the input time
    const [timePart, modifier] = time.split(" "); // Split into time part and AM/PM
    let [hours, minutes] = timePart.split(":").map(Number); // Split hours and minutes

    // Handle AM/PM
    if (modifier === "PM" && hours < 12) {
      hours += 12; // Convert PM hours to 24-hour format
    } else if (modifier === "AM" && hours === 12) {
      hours = 0; // Midnight case
    }

    // Format hours, minutes, and seconds
    const formattedTime = `${hours.toString().padStart(2, "0")}:${(minutes || 0)
      .toString()
      .padStart(2, "0")}:00`;
    return formattedTime;
  }

  openContractDialog(): void {
    const dialogRef = this.dialog.open(ContractDialogComponent, {
      width: "60vw",
      height: "80vh",
      data: {
        admin: this.service._AdminContract,
        provider: this.service._ProviderContract,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.acceptContract = true;
      } else {
        this.acceptContract = false;
      }
    });
  }

  // Method to calculate total
  calculateTotal() {
    const initialPayment =
      parseFloat(this.submitForm.get("payment.minValue")?.value) || 0;
    const priceEuro =
      parseFloat(this.submitForm.get("payment.maxValue")?.value) || 0;
    const minPrice = (initialPayment * priceEuro) / 100;
    this.submitForm
      .get("payment.amount")
      ?.setValue(minPrice, { emitEvent: false });
  }

  CreatePayment() {
    // Share the booking data before proceeding with payment
    this.shareData(); // Call shareData here to share the booking data

    // Get total value from the form
    const amount = this.submitForm.get("payment.amount")?.value; // Reference the correct form group

    // Ensure minValue and maxValue are properly set from the form
    this.minValue = parseFloat(this.submitForm.get("payment.minValue")?.value);
    this.maxValue = parseFloat(this.submitForm.get("payment.maxValue")?.value);

    // Check if the total is between minValue and maxValue
    if (
      amount < (this.minValue * this.maxValue) / 100 ||
      amount > this.maxValue ||
      amount <= 0
    ) {
      console.warn("Total must be between minimum and maximum values:", amount);
      return; // Exit the method if the total is not in the valid range
    }

    const paymentData = {
      total: amount,
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
        console.error("Payment Error:", error);
      },
    });
  }

  private initializeTimeOptions() {
    this.timeOptions = [];
    this.startHour = parseInt(this.service.startTime.split(":")[0], 10); // Start from 9 AM
    this.endHour = parseInt(this.service.endTime.split(":")[0], 10); // End at 6 PM
    for (let hour = this.startHour; hour < this.endHour; hour++) {
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour; // Convert to 12-hour format
      //console.log(`${displayHour} ${amPm}`);
      if (!this.timeBooked.includes(`${displayHour} ${amPm}`)) {
        this.timeOptions.push(`${displayHour} ${amPm}`);
      }
    }
  }

  end: number = 0;
  private updateEndTimeOptions(startTime: string) {
    // Reset end time options
    this.endTimeOptions = [];

    if (!startTime) {
      return; // No start time selected
    }
    // Get the hour from the start time // 10 AM
    const [hourStr, period] = startTime.split(" ");
    let startHour = parseInt(hourStr);
    if (period === "PM" && startHour !== 12) {
      startHour += 12;
    } else if (period === "AM" && startHour === 12) {
      startHour = 24;
    }

    let startTimeEdited = [];
    for (let index = 0; index < this.timeOptions.length; index++) {
      const [hourCur, period] = this.timeOptions[index].split(" ");
      let hour = parseInt(hourCur);
      if (period === "PM" && hour !== 12) {
        hour += 12;
      } else if (period === "AM" && hour === 12) {
        hour = 0;
      }
      if (startHour <= hour) {
        startTimeEdited.push(hour === 0 ? 24 : hour);
      }
    }

    for (let index = 0; index < startTimeEdited.length; index++) {
      const currentHour = index;
      const nextHour = index + 1;
      let hourCur = startTimeEdited[currentHour];
      let hourNext = startTimeEdited[nextHour];
      if (hourCur + 1 === hourNext) {
        this.endHour = hourNext;
      } else if (hourCur == startTimeEdited[startTimeEdited.length - 1]) {
        this.endHour = hourCur + 1;
        break;
      } else if (hourNext > hourCur + 1) {
        this.endHour = hourNext - 1;
        break;
      } 
    }

    for (let hour = startHour + 1; hour <= this.endHour; hour++) {
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour; 
      this.endTimeOptions.push(`${displayHour} ${amPm}`);
    }
  }

  onTimeSelected(selectedDate: Date) {
    this.timeBooked = [];
    this.services = [];
    this.date = "";
    this.bookedQuantity = 0;
    this.EndTimeForLastBook = 0;
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
            for (let hour = startTime; hour < endTime; hour++) {
              const amPm = hour >= 12 ? "PM" : "AM";
              const displayHour = hour > 12 ? hour - 12 : hour;
              this.timeBooked.push(`${displayHour} ${amPm}`);
            }
            this.bookedQuantity += service.quantity;
          }
          this.maxQuantity = this.service.quantity - this.bookedQuantity;
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
