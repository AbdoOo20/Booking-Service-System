import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";

@Injectable({
    providedIn: "root",
})
export class DecodingTokenService {
    decoded: string;
    constructor() { }

    getUserIdFromToken() {
        let token;
        if (sessionStorage.getItem("token"))
            token = sessionStorage.getItem("token");
        if (!token)
         token = localStorage.getItem("token");
        if (token) {
            this.decoded = jwtDecode(token);
            return this.decoded[
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            ];
        } else return null;
    }

    getUserNameFromToken() {
        const token = localStorage.getItem("token");
        if (token) {
            this.decoded = jwtDecode(token);
            return this.decoded[
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            ];
        } else return null;
    }
}
