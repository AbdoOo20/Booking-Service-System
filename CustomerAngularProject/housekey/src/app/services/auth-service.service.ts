import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
    providedIn: "root",
})
export class AuthServiceService {
    private loggedIn = new BehaviorSubject<boolean>(this.isLoggedIn());

    isLoggedIn$ = this.loggedIn.asObservable();
    constructor() { }
    private isLoggedIn(): boolean {
        if (localStorage.getItem("token"))
            return !!localStorage.getItem("token");
        else (sessionStorage.getItem("token"))
        return !!sessionStorage.getItem("token");
    }

    login(token: string): void {
        localStorage.setItem("token", token);
        this.loggedIn.next(true);
    }
    logout(): void {
        localStorage.clear();
        sessionStorage.clear();
        this.loggedIn.next(false);
    }
}
