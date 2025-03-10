import {
  Component,
  ElementRef,
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
import { MatSelectChange, MatSelectModule } from "@angular/material/select";
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
import { CommonModule } from "@angular/common";
import { PayPalService } from "@services/pay-pal.service";
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
import { PaymentIncome } from "../../common/interfaces/payment-income";
import { PaymentsService } from "@services/payments.service";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import {
  ConfirmDialogComponent,
  ConfirmDialogModel,
} from "@shared-components/confirm-dialog/confirm-dialog.component";
import { AlertDialogComponent } from "@shared-components/alert-dialog/alert-dialog.component";

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
    MatProgressSpinnerModule,
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

  //Get Payment Incomes Initialization
  public PaymentMethods: PaymentIncome[] = [];

  // Booking Data
  bookingData: any;
  public eventBookingDate: string;
  public startBookingTime: string;
  public endBookingTime: string;
  CustomerIDFromToken: any;
  paymentMethodId: number;
  isPaymentMethodSelected: boolean = false;

  //Quantity
  public QuantityForPayment: number;

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
    private router: Router,
    private PaymentsService: PaymentsService
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
    localStorage.setItem('firstCall', 'false');
    if (!this.decodingService.getToken()) {
      const targetPage = "/login";
      this.router.navigate([targetPage]);
    }
    this.activatedRoute.paramMap.subscribe((params) => {
      this.serviceID = Number(params.get("id")); // استقبال الـ id
      //this.serviceID); // طباعة الـ id
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

    //Get Payment methodes:
    this.PaymentsService.getAllPaymentIncoms().subscribe({
      next: (data) => {
        this.PaymentMethods = data;
      },
    });
    this.submitForm = this.fb.group({
      booking: this.fb.group({
        service: ["", Validators.required],
        priceEuro: ["", Validators.required],
        eventDate: ["", Validators.required],
        startTime: ["", Validators.required],
        endTime: ["", Validators.required],
        quantity: ["", [Validators.required, Validators.min(0)]],
      }), //as
      payment: this.fb.group({
        amount: ["", [Validators.required, Validators.maxLength(5)]],
        maxValue: ["", [Validators.required, Validators.maxLength(5)]],
        minValue: ["", [Validators.required, Validators.maxLength(5)]],
        paymentMethod: ["", Validators.required],
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
        // this.submitForm.patchValue({
        //   payment: {
        //     minValue: this.service.initialPayment * this.QuantityForPayment,
        //     maxValue: this.service.priceForTheCurrentDay * this.QuantityForPayment,
        //     paymentMethod: [1, Validators.required],
        //   },
        // });
        this.hasQuantity = this.service.quantity > 0 ? true : false;
        //this.calculateTotal();
        this.initializeTimeOptions();

        const quantityControl = this.submitForm.get("booking.quantity");
        if (this.service.quantity > 0) {
          // quantityControl?.setAsyncValidators(
          //   this.asyncQuantityValidator(this.service.quantity)
          // );
          quantityControl?.setValidators([
            Validators.required,
            Validators.min(1),
          ]);
        } else {
          //quantityControl?.clearAsyncValidators();
          quantityControl?.clearValidators();
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
    //selectedDate);
    //"----------------------------------------");
    const formatedEvantDate = this.formatEventDate(selectedDate);
    //formatedEvantDate);
    //"-----------------------------------------------------");
    localStorage.setItem("NewDataFormat", formatedEvantDate);

    this.bookingData = {
      bookId: 0,
      eventDate: selectedDate,
      startTime: convertedStartTime,
      endTime: convertedEndTime,
      initialPaymentPercentage: this.service.initialPayment,
      status: "",
      quantity: Number(this.submitForm.get("booking.quantity").value),
      price: parseFloat(this.submitForm.get('payment.maxValue').value),
      cashOrCashByHandOrInstallment: "",
      bookDate: new Date().toISOString(),
      type: "Service",
      customerId: this.CustomerIDFromToken,
      serviceId: this.serviceID,
      paymentIncomeId: this.paymentMethodId,
    };

    //this.bookingData);

    // Save data in local storage
    localStorage.setItem("bookingData", JSON.stringify(this.bookingData));

    // Set data in SharedService
    this.sharedService.setData(this.bookingData);
    this.NewBooking.setData(this.bookingData);
  }

  formatEventDate(dateString: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: "numeric",
      month: "long", // 'short' for abbreviated month names
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      hour12: true, // Set to false for 24-hour format
    };
    return new Date(dateString).toLocaleString("en-US", options);
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
      this.QuantityForPayment = Math.abs(Number(this.submitForm.get("booking.quantity").value));
      console.log(this.QuantityForPayment);

      this.submitForm.patchValue({
        payment: {
          minValue: ((this.service.initialPayment * this.service.priceForTheCurrentDay) / 100) * this.QuantityForPayment,
          maxValue: this.service.priceForTheCurrentDay * this.QuantityForPayment,
          amount: ((this.service.initialPayment * this.service.priceForTheCurrentDay) / 100) * this.QuantityForPayment,
          paymentMethod: [1, Validators.required],
        },
      });


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

  // Declare a variable to store the selected method ID
  selectedPaymentMethodId: number | null = null;

  onSelectMethod(methodId: number): void {
    this.selectedPaymentMethodId = methodId;
    //"Selected Payment Method ID:", this.selectedPaymentMethodId);
  }

  // Custom validator function
  amountRangeValidator = (min: number, max: number) => {
    return (control: AbstractControl): ValidationErrors | null => {
      const amount = control.value;
      if (amount < min || amount > max) {
        return { outOfRange: true };
      }
      return null;
    };
  };
  loading: boolean = false;

  CreatePayment() {
    // Step 1: Retrieve form values and validate the amount
    const amount = this.submitForm.get('payment.amount')?.value;
    this.minValue = parseFloat(this.submitForm.get('payment.minValue')?.value);
    this.maxValue = parseFloat(this.submitForm.get('payment.maxValue')?.value);
    console.log(this.minValue);
    console.log(amount);

    if (
      amount < this.minValue ||
      amount > this.maxValue ||
      amount <= 0
    ) {
      console.warn('Total must be between minimum and maximum values:', amount);

      // Open an alert dialog to notify the user about the invalid amount
      this.dialog.open(AlertDialogComponent, {
        maxWidth: "500px",
        data: "The amount must be between the specified minimum and maximum values.",
      });

      return; // Exit the function if validation fails
    }

    // Step 2: Open a confirmation dialog for saving the bank account
    const dialogData = new ConfirmDialogModel(
      "Confirm",
      "Save Your Account Bank !!"
    );
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      minWidth: "500px",
      maxWidth: "500px",
      data: dialogData,
    });

    // Step 3: Wait for the user's response and proceed with the payment
    dialogRef.afterClosed().subscribe((result) => {
      localStorage.setItem('SetBankAccount', result ? 'true' : 'false');

      if (!result) {
        console.warn("User canceled bank account saving.");
        // Continue with the payment even if the user cancels saving the account
      }

      // Set loading to true when the payment process starts
      this.loading = true;

      // Share the booking data
      this.shareData();

      // Prepare payment data
      const paymentData = {
        total: amount,
        currency: "USD",
        description: "New Transaction",
        returnUrl: "https://lilynight.vercel.app/confirmation",
        cancelUrl: "https://lilynight.vercel.app/submit-property",
      };

      // Proceed with PayPal payment
      this.PayPal.addPayment(paymentData).subscribe({
        next: (response) => {
          localStorage.setItem('firstCall', 'true');
          window.location.href = response.approvalUrl;
        },
        error: (error) => {
          console.error("Payment Error:", error);
          this.loading = false;
        },
      });
    });
  }

  onSelectionChange2(event: MatSelectChange) {
    //"Selected Payment Method:", event.value);
    // Update the selected payment method ID and mark it as selected
    this.paymentMethodId = event.value;
    this.isPaymentMethodSelected = !!event.value; // Ensure it's set only if valid
  }

  private initializeTimeOptions() {
    this.timeOptions = [];
    this.startHour = parseInt(this.service.startTime.split(":")[0], 10); // Start from 9 AM
    this.endHour = parseInt(this.service.endTime.split(":")[0], 10); // End at 6 PM
    for (let hour = this.startHour; hour < this.endHour; hour++) {
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour; // Convert to 12-hour format
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
        this.endHour = hourCur + 1;
        break;
      }
    }
    for (let hour = startHour + 1; hour <= this.endHour; hour++) {
      const amPm = hour >= 12 ? "PM" : "AM";
      const displayHour = hour > 12 ? hour - 12 : hour;
      this.endTimeOptions.push(`${displayHour} ${amPm}`);
    }
    // if (startHour + 1 === this.endHour) {

    // } else {
    //   for (let hour = startHour + 1; hour <= this.endHour; hour++) {
    //     const amPm = hour >= 12 ? "PM" : "AM";
    //     const displayHour = hour > 12 ? hour - 12 : hour;
    //     this.endTimeOptions.push(`${displayHour} ${amPm}`);
    //   }
    // }
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
      //this.submitForm.value);
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
}
