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
    private userService: UserService
  ) { }

  ngOnInit() {
    this.property = {
      customerId:'3'
    
    };
    
    this.infoForm = this.formBuilder.group({
      customerId: '3',
      name: ['', Validators.compose([Validators.required, Validators.minLength(3)])],
      email: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$')  // Email regex pattern
      ])],
      city: ['', Validators.compose([Validators.required, Validators.minLength(3)])],
      ssn: ['', Validators.compose([Validators.required, Validators.minLength(14), Validators.maxLength(14)])],
      phone: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^(009665|9665|\\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$') // Phone regex pattern
      ])],
      alternativePhone: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^(009665|9665|\\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$') // Phone regex pattern
      ])]
      // bankAccountEmail: ['', Validators.compose([Validators.required, Validators.pattern('^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$')])] // Update balance to bank account email
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
    this.userService.getUserData().subscribe(
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
  private updateUserData(): void {
    console.log(this.infoForm);

    this.userService.updateCustomerData('3', this.infoForm.value).subscribe(

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


  public onInfoFormSubmit(values: any): void {
    if (this.infoForm.valid) {
      console.log("Submitting data:", this.infoForm.value); // إضافة هذا السطر
      this.updateUserData();
    } else {
      console.log("Form is invalid:", this.infoForm.errors); // إضافة هذا السطر
    }
  }


  public onPasswordFormSubmit(values: any): void {
    if (this.passwordForm.valid) {
      this.userService.changePassword(values).subscribe(
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
