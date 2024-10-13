import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { emailValidator, matchingPasswords } from '../../theme/utils/app-validators';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatButtonModule,
    MatSlideToggleModule,
    FlexLayoutModule,
    MatSnackBarModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})

export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  hidePassword = true;
  hideConfirmPassword = true;
  errorMessage: string = '';
  successMessage: string = '';  // Added success message
  serverValidationErrors: any = {};

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(75)
        ]
      ],
      username: [
        '',[
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]
      ],
      email: [
        '',
        [
          Validators.required,
          Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)
        ]
      ],
      phone: [
        '',
        [
          Validators.required,
          Validators.pattern(/^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$/)
        ]
      ],
      alternativePhone: [
        '',
        [
          Validators.required,
          Validators.pattern(/^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$/)
        ]
      ],
      ssn: [
        '',
        [
          Validators.required,
          Validators.pattern(/^[12]\d{9}$/)
        ]
      ],
      city: [
        '',[
        Validators.required,
        Validators.minLength(3)]
      ],
      password: [
        '',[
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/)
      ]],
      confirmPassword: [
        '',[
        Validators.required,
      ]]
    }, { validator: this.passwordMatchValidator });
  }

  
  // passwordMatchValidator(form: FormGroup) {
  //   return form.controls['password'].value === form.controls['confirmPassword'].value
  //     ? null : { 'mismatchedPasswords': true };
  // }

  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password');
    const confirmPassword = formGroup.get('confirmPassword');

    if (confirmPassword?.errors && !confirmPassword.errors['passwordMismatch']) {
      return; // If another error already exists, skip further checks
    }

    if (password?.value !== confirmPassword?.value) {
      confirmPassword?.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null); // Clear error if passwords match
    }
  }

  getErrorMessage(field: string) {
    const control = this.registerForm.get(field);

    if (this.serverValidationErrors[field]) {
      return this.serverValidationErrors[field];
    }

    if (control?.hasError('required')) {
      return `${field.charAt(0).toUpperCase() + field.slice(1)} is required`;
    }
    if (control?.hasError('minlength')) {
      return `Minimum length for ${field} is ${control.errors?.minlength.requiredLength} characters`;
    }
    if (control?.hasError('maxlength')) {
      return `Maximum length for ${field} is ${control.errors?.maxlength.requiredLength} characters`;
    }
    if (control?.hasError('pattern')) {
      if (field === 'email') return 'Invalid email address';
      if (field === 'phone' || field === 'alternativePhone') return 'Invalid Saudi phone number';
      if (field === 'ssn') return 'Invalid Saudi SSN number';
      if (field === 'password') return 'Password must contain uppercase, lowercase letters, digits, and special characters';

    }
    if (control?.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return '';
  }




  onRegisterFormSubmit(): void {
    if (this.registerForm.invalid) {
      // If form is invalid, show error message
      this.snackBar.open('Please fill in all required fields correctly.', '×', {
        panelClass: 'error',
        verticalPosition: 'top',
        duration: 3000
      });
      return;
    }

    const registerData = {
      Name: this.registerForm.value.name,
      Username: this.registerForm.value.username,
      Email: this.registerForm.value.email,
      Phone: this.registerForm.value.phone,
      AlternativePhone: this.registerForm.value.alternativePhone,
      SSN: this.registerForm.value.ssn,
      City: this.registerForm.value.city,
      Password: this.registerForm.value.password
    };

    this.http.post('http://localhost:5285/api/Account/Register', registerData).subscribe(
      (response) => {
        // Successful registration message
        console.log('Registration successful:', response);
        this.snackBar.open('You registered successfully! Please login now.', '×', {
          panelClass: 'success',
          verticalPosition: 'top',
          duration: 3000
        });
        setTimeout(() => {
          this.router.navigate(['/login']);  // Redirect after a delay
        }, 5000); // 3 seconds delay
      },
      (error: HttpErrorResponse) => {
        console.error('API Error:', error);
        if (error.status === 400 && error.error && error.error.errors) {
          // Handle validation errors from the server
          const validationErrors = error.error.errors;
          console.error('Validation errors:', validationErrors);

          let errorMessage = 'Registration failed. Please check the following fields:\n';
          Object.keys(validationErrors).forEach((field) => {
            errorMessage += `${field}: ${validationErrors[field].join(', ')}\n`;
          });
          this.snackBar.open(errorMessage, '×', {
            panelClass: 'error',
            verticalPosition: 'top',
            duration: 5000
          });
        } else {
          // General error message if other error occurs
          this.snackBar.open('Registration failed. Please try again.', '×', {
            panelClass: 'error',
            verticalPosition: 'top',
            duration: 3000
          });
        }
      }
    );
  }
}