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
import { response } from 'express';

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
  public registerForm: FormGroup;
  public hidePassword = true;
  public hideConfirmPassword = true;
  public cities: string[] = [
    'Riyadh', 'Jeddah', 'Mecca', 'Medina', 'Dammam', 'Khobar', 'Tabuk', 'Abha', 'Najran'
  ];

  constructor(
    public fb: FormBuilder,
    public router: Router,
    public snackBar: MatSnackBar,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.registerForm = this.fb.group({
      CustomerId:'eslam',
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
      email: ['', [Validators.required, emailValidator]],
      phone: ['', [Validators.required, Validators.pattern(/^05\d{8}$/)]],
      alternativePhone: ['', [Validators.required, Validators.pattern(/^05\d{8}$/)]],
      ssn: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      city: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/),
        ],
      ],
      confirmPassword: ['', Validators.required],
    }, { validator: matchingPasswords('password', 'confirmPassword') });
  }

  onRegisterFormSubmit(): void {
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;
  
      console.log('Form data being sent:', formData);
      //debugger; // Debugging tool to inspect form data
  
      this.http.post('http://localhost:5285/api/Account/Register', formData).subscribe(
        (response) => {
          // Success handling
          console.log('Registration successful:', response);
          this.snackBar.open('You registered successfully!', '×', {
            panelClass: 'success',
            verticalPosition: 'top',
            duration: 3000
          });
          this.router.navigate(['/login']);
        },
        (error: HttpErrorResponse) => {
          // Error handling
          console.error('API Error:', error);
          if (error.status === 400 && error.error && error.error.errors) {
            // Log validation errors for debugging
            const validationErrors = error.error.errors;
            console.error('Validation errors:', validationErrors);
  
            // Show specific validation error messages to the user
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
            // General error handling
            this.snackBar.open('Registration failed. Please try again.', '×', {
              panelClass: 'error',
              verticalPosition: 'top',
              duration: 3000
            });
          }
        }
      );
    } else {
      // Form is invalid, show an error message
      this.snackBar.open('Please fill in all required fields correctly.', '×', {
        panelClass: 'error',
        verticalPosition: 'top',
        duration: 3000
      });
    }
  }
  

  // Error message handling for validation
  getErrorMessage(field: string): string {
    const control = this.registerForm.get(field);
    if (control?.hasError('required')) {
      return `${field} is required`;
    }
    if (control?.hasError('minlength')) {
      return `${field} must be at least ${control.errors?.['minlength'].requiredLength} characters`;
    }
    if (control?.hasError('maxlength')) {
      return `${field} must be less than ${control.errors?.['maxlength'].requiredLength} characters`;
    }
    if (control?.hasError('pattern')) {
      if (field === 'phone' || field === 'alternativePhone') {
        return 'Invalid Saudi phone number';
      }
      if (field === 'ssn') {
        return 'Invalid SSN, must be a 10-digit Saudi number';
      }
      if (field === 'password') {
        return 'Password must contain at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character';
      }
    }
    return '';
  }
}