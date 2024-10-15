import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
    providedIn: "root",
})
export class AuthServiceService {
    private loggedIn = new BehaviorSubject<boolean>(this.isLoggedIn());

    isLoggedIn$ = this.loggedIn.asObservable();
    constructor() {}
    private isLoggedIn(): boolean {
        return !!localStorage.getItem("token");
    }

    login(token: string): void {
        localStorage.setItem("token", token);
        this.loggedIn.next(true);
    }
    logout(): void {
        localStorage.clear();
        this.loggedIn.next(false);
    }
}
