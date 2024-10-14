import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { Router, RouterModule } from "@angular/router";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { AuthServiceService } from "@services/auth-service.service";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatTooltipModule,
    FlexLayoutModule,
  ],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage: string = "";
  hide: boolean = true;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private snackBar: MatSnackBar, // For displaying messages
    private authService: AuthServiceService
  ) {}

  ngOnInit(): void {
    // Initialize login form
    this.loginForm = this.fb.group({
      username: [
        "",
        [
          Validators.required,
          Validators.maxLength(100),
          Validators.minLength(3),
        ],
      ],
      password: [
        "",
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/
          ),
        ],
      ],
      rememberMe: [false],
    });
  }
  // Handle form submission
  onLoginFormSubmit() {
    if (this.loginForm.invalid) {
      // If form is invalid, show a message
      this.snackBar.open("Please fill in all required fields correctly.", "×", {
        panelClass: "error",
        verticalPosition: "top",
        duration: 3000,
      });
      return;
    }

    const loginData = {
      UserName: this.loginForm.value.username,
      Password: this.loginForm.value.password,
    };

    // Call the API for login
    this.http
      .post("http://localhost:18105/api/Account/Login", loginData)
      .subscribe({
        next: (response: any) => {
          if (response.token) {
            // Save the token and navigate to the home page
            this.authService.login(response.token);
            //localStorage.setItem('token', response.token);
            this.router.navigate(["/home"]);
            this.snackBar.open("Login successful!", "×", {
              panelClass: "success",
              verticalPosition: "top",
              duration: 3000,
            });
          }
        },
        error: (errorResponse: HttpErrorResponse) => {
          console.error("Login Error:", errorResponse);
          if (errorResponse.error) {
            if (errorResponse.error.Message) {
              // Handle the ModelState error messages returned from the API
              let errorMessage = "Login failed. Please check the following:\n";
              const validationErrors = errorResponse.error.Message;

              // Constructing error message based on the API response
              for (const key in validationErrors) {
                if (validationErrors.hasOwnProperty(key)) {
                  errorMessage += `${key}: ${validationErrors[key].join(
                    ", "
                  )}\n`;
                }
              }

              this.snackBar.open(errorMessage, "×", {
                panelClass: "error",
                verticalPosition: "top",
                duration: 5000,
              });
            } else {
              // Handle specific error messages from the API
              if (errorResponse.error["User Blocked"]) {
                this.errorMessage =
                  "Your account is blocked. Please contact customer service.";
              } else {
                this.errorMessage = "Invalid username or password.";
              }
              // Display error in snackbar
              this.snackBar.open(this.errorMessage, "×", {
                panelClass: "error",
                verticalPosition: "top",
                duration: 3000,
              });
            }
          } else {
            this.errorMessage =
              "An unexpected error occurred. Please try again later.";
            // Display error in snackbar
            this.snackBar.open(this.errorMessage, "×", {
              panelClass: "error",
              verticalPosition: "top",
              duration: 3000,
            });
          }
        },
      });
  }

  get username() {
    return this.loginForm.get("username");
  }

  get password() {
    return this.loginForm.get("password");
  }
}
