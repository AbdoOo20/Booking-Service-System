import { Component, Inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-contract-dialog",
  standalone: true,
  imports: [FormsModule, MatCheckboxModule],
  templateUrl: "./contract-dialog.component.html",
  styleUrls: ["./contract-dialog.component.css"],
})
export class ContractDialogComponent {
  isAccepted: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<ContractDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  get admin() {
    return this.data.admin;
  }

  get provider() {
    return this.data.provider;
  }

  acceptContract(): void {
    this.dialogRef.close(this.isAccepted);
  }
}
