import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";

@Injectable({
    providedIn: "root",
})
export class DecodingTokenService {
    constructor() {}

    getUserIdFromToken() {
        const token = localStorage.getItem("token");
        const decoded = jwtDecode(token);
        return decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ];
    }
}
