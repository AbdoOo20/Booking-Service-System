import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { emailValidator, matchingPasswords } from '../../../theme/utils/app-validators';
import { MatInputModule } from '@angular/material/input';
import { InputFileModule } from '../../../theme/components/input-file/input-file.module';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatButtonModule } from '@angular/material/button';
import { UserService } from '../profile/user.service';
import { CommonModule } from '@angular/common';
import { DecodingTokenService } from '@services/decoding-token.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule, // تأكد من إضافته هن
    ReactiveFormsModule,
    MatSnackBarModule,
    MatInputModule,
    InputFileModule,
    FlexLayoutModule,
    MatButtonModule,
  ],
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {
  public infoForm: FormGroup;
  public passwordForm: FormGroup;
  public property: any;

  constructor(
    public formBuilder: FormBuilder,
    public snackBar: MatSnackBar,
    private userService: UserService,
    private DecodeService: DecodingTokenService,
  ) { }

  ngOnInit() {
    this.property = {
      customerId: this.DecodeService.getUserIdFromToken(),
    };

    this.infoForm = this.formBuilder.group({
      customerId: this.DecodeService.getUserIdFromToken(),
      name: ['', Validators.compose([Validators.required, Validators.minLength(3)])],
      email: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$')  // Email regex pattern
      ])],
      city: ['', Validators.compose([Validators.required, Validators.minLength(3)])],
      ssn: [
        '',
        [
          Validators.required,
          Validators.pattern(/^[12]\d{9}$/)
        ]
      ],
      phone: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^(009665|9665|\\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$') // Phone regex pattern
      ])],
      alternativePhone: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^(009665|9665|\\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$') // Phone regex pattern
      ])],
      bankAccountEmail: ['', Validators.compose([Validators.required, Validators.pattern('^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$')])] // Update balance to bank account email
    });

    this.passwordForm = this.formBuilder.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', Validators.required]
    }, { validator: matchingPasswords('newPassword', 'confirmNewPassword') });

    // Load user data
    this.loadUserData();
  }

  private loadUserData(): void {
    this.userService.getUserData(this.DecodeService.getUserIdFromToken()).subscribe(
      (data) => {
        // Fill the form with user data
        this.infoForm.patchValue(data);
      },
      (error) => {
        this.snackBar.open('Failed to load user data!', '×', { duration: 3000, panelClass: 'error' });
      }
    );
  }

  // إضافة دالة لتحديث بيانات المستخدم
  // (response) => {
  //  this.snackBar.open('تم تحديث بيانات المستخدم بنجاح!', '×', { duration: 3000 });
  // },
  // (error) => {
  //  console.log("Error during update:", error); // إضافة هذا السطر
  //  this.snackBar.open('فشل في تحديث بيانات المستخدم!', '×', { duration: 3000, panelClass: 'error' });
  // }

  public onInfoFormSubmit(values: any): void {
    if (values) {
      console.log("Submitting data:", values); // إضافة هذا السطر
      this.updateUserData(this.DecodeService.getUserIdFromToken(),values);
    } else {
      console.log("Form is invalid:", this.infoForm.errors); // إضافة هذا السطر
    }
  }
  private updateUserData(id:string , data: any): void {
    console.log(data);
    this.userService.updateCustomerData(id, data).subscribe(
      // إضافة دالة لتحديث بيانات المستخدم
      (response) => {
        this.snackBar.open('successfully updated!', '✓', { duration: 3000 });
      },
      (error) => {
        console.log("Error during update:", error); // إضافة هذا السطر
        this.snackBar.open('Failed to update !', '×', { duration: 3000, panelClass: 'error' });
      }
    );
  }



  public onPasswordFormSubmit(values: any): void {
    if (values) {
      this.userService.changePassword(this.DecodeService.getUserIdFromToken(), values).subscribe(
        response => {
          this.snackBar.open('Password is Updated !', '×', { duration: 3000, panelClass: 'success' });
        },
        error => {
          this.snackBar.open('Failed to change password!', '×', { duration: 3000, panelClass: 'error' });
        }
      );
    }
  }

  blockCustomer(id: string) {
    this.userService.blockUser(id).subscribe(
      (response) => {
        console.log("User blocked successfully");
        this.snackBar.open('User blocked successfully!', '✓', { duration: 3000 });
        // يمكنك هنا تحديث الـ UI أو عرض رسالة نجاح
      },
      (error) => {
        console.error("Failed to block user", error);
        this.snackBar.open('Failed to block user!', '×', { duration: 3000, panelClass: 'error' });
        // يمكنك هنا عرض رسالة خطأ
      }
    );
  }

}
