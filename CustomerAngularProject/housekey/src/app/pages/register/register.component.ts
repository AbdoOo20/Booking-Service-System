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
import { HttpClient } from '@angular/common/http';

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
// export class RegisterComponent implements OnInit {
//   public registerForm: FormGroup;
//   public hide = true;
//   public userTypes = [
//     { id: 1, name: 'Agent' },
//     { id: 2, name: 'Agency' },
//     { id: 3, name: 'Buyer' }
//   ];
//   constructor(public fb: FormBuilder, public router: Router, public snackBar: MatSnackBar) { }

//   ngOnInit() {
//     this.registerForm = this.fb.group({
//       userType: ['', Validators.required],
//       username: ['', Validators.compose([Validators.required, Validators.minLength(6)])],
//       email: ['', Validators.compose([Validators.required, emailValidator])],
//       password: ['', Validators.required],
//       confirmPassword: ['', Validators.required],
//       receiveNewsletter: false
//     }, { validator: matchingPasswords('password', 'confirmPassword') });
//   }

//   public onRegisterFormSubmit(): void {
//     if (this.registerForm.valid) {
//       console.log(this.registerForm.value);
//       this.snackBar.open('You registered successfully!', '×', { panelClass: 'success', verticalPosition: 'top', duration: 3000 });
//     }
//   }
// }



export class RegisterComponent implements OnInit {
  public registerForm: FormGroup;
  public hide = true;
  public hide1 = true;
  public cities = ['Riyadh', 'Jeddah', 'Mecca', 'Medina', 'Dammam']; // Example Saudi cities

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private snackBar: MatSnackBar,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(6)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$/)]], // Saudi phone number format
      alternativePhone: ['', [Validators.pattern(/^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$/)]],
      ssn: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]], // Saudi SSN pattern
      city: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, { validator: matchingPasswords('password', 'confirmPassword') });
  }

  public onRegisterFormSubmit(): void {
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;
      const customerData = {
        Username: formData.username,
        Name: formData.name,
        Email: formData.email,
        Phone: formData.phone,
        AlternativePhone: formData.alternativePhone,
        SSN: formData.ssn,
        City: formData.city,
        Password: formData.password
      };

      this.http.post('http://localhost:5285/api/Account/Register', customerData).subscribe(
        () => {
          this.router.navigate(['/home']);
          this.snackBar.open('Welcome! You have successfully registered!', '×', { panelClass: 'success', verticalPosition: 'top', duration: 3000 });
        },
        (error) => {
          this.snackBar.open('Registration failed. Please try again.', '×', { panelClass: 'error', verticalPosition: 'top', duration: 3000 });
        }
      );
    } else {
      this.snackBar.open('Please correct the errors in the form', '×', { panelClass: 'error', verticalPosition: 'top', duration: 3000 });
    }
  }
}