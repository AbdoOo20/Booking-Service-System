import { Injectable } from "@angular/core";
import { jwtDecode } from "../../../node_modules/jwt-decode";

@Injectable({
  providedIn: "root",
})
export class DecodingTokenService {
  private token: string | null = null;
  private decoded: { [key: string]: any } | null = null;

  constructor() {}

  public getToken(): string | null {
    if (!this.token) {
      this.token =
        sessionStorage.getItem("token") || localStorage.getItem("token");
    }
    return this.token;
  }

  private decodeToken(): void {
    if (this.decoded || !this.getToken()) return;
    try {
      this.decoded = jwtDecode<{ [key: string]: any }>(this.token!);
    } catch (error) {
      console.error("Invalid token:", error);
      this.decoded = null;
    }
  }

  getUserIdFromToken(): string | null {
    this.decodeToken();
    return (
      this.decoded?.[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ] ?? null
    );
  }

  getUserNameFromToken(): string | null {
    this.decodeToken();
    return (
      this.decoded?.[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
      ] ?? null
    );
  }

  clearToken(): void {
    this.token = null;
    this.decoded = null;
  }
}
