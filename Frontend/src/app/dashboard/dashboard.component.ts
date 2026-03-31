import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';
import { QuantityService } from '../shared/services/quantity.service';
import { AuthResponse } from '../shared/models/auth.models';

type MeasurementType = 'length' | 'weight' | 'temperature' | 'volume';
type Operation = 'convert' | 'add' | 'subtract' | 'compare';

// ✅ ONLY units that exist in backend enums
const UNITS: Record<MeasurementType, string[]> = {
  length:      ['Inches', 'Feet', 'Yards', 'Centimeters','MiliMeters'],
  weight:      ['Grams', 'Kilograms', 'Pound'],
  temperature: ['Celsius', 'Fahrenheit', 'Kelvin'],
  volume:      ['Litre', 'MilliLiter', 'Gallon']
};

const TYPE_ICONS: Record<MeasurementType, string> = {
  length: '📏', weight: '⚖️', temperature: '🌡️', volume: '🧪'
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  currentUser: AuthResponse | null = null;

  selectedType: MeasurementType = 'length';
  selectedOp: Operation = 'convert';

  convertForm!: FormGroup;
  dualForm!: FormGroup;

  result: string | null = null;
  resultUnit: string = '';
  loading = false;
  errorMsg = '';

  readonly types: MeasurementType[] = ['length', 'weight', 'temperature', 'volume'];
  readonly ops: { key: Operation; label: string; icon: string }[] = [
    { key: 'convert',  label: 'Convert',  icon: '🔄' },
    { key: 'add',      label: 'Add',      icon: '➕' },
    { key: 'subtract', label: 'Subtract', icon: '➖' },
    { key: 'compare',  label: 'Compare',  icon: '⚡' }
  ];

  typeIcons = TYPE_ICONS;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private quantityService: QuantityService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.buildForms();
  }

  get units(): string[] {
    return UNITS[this.selectedType];
  }

  selectType(t: MeasurementType): void {
    this.selectedType = t;
    this.result = null;
    this.errorMsg = '';
    this.buildForms();
  }

  selectOp(op: Operation): void {
    this.selectedOp = op;
    this.result = null;
    this.errorMsg = '';
  }

  private buildForms(): void {
    const units = UNITS[this.selectedType];

    this.convertForm = this.fb.group({
      value:    [null, [Validators.required]],
      fromUnit: [units[0], Validators.required],
      toUnit:   [units[1], Validators.required]
    });

    this.dualForm = this.fb.group({
      value1: [null, [Validators.required]],
      unit1:  [units[0], Validators.required],
      value2: [null, [Validators.required]],
      unit2:  [units[1], Validators.required]
    });
  }

  onSubmit(): void {
    this.result = null;
    this.errorMsg = '';

    const activeForm = this.selectedOp === 'convert' ? this.convertForm : this.dualForm;
    if (activeForm.invalid) {
      activeForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    if (this.selectedOp === 'convert') {
      const v = this.convertForm.value;
      this.quantityService.convert({
        value: v.value,
        fromUnit: v.fromUnit,
        toUnit: v.toUnit,
        type: this.selectedType
      }).subscribe({
        next: (res) => {
          this.loading = false;
          this.result = res.result.toString();
          this.resultUnit = res.unit;
        },
        error: (err) => {
          this.loading = false;
          this.errorMsg = err.error?.message || err.error || 'Operation failed.';
        }
      });
      return;
    }

    const v = this.dualForm.value;
    const payload = {
      value1: v.value1, unit1: v.unit1,
      value2: v.value2, unit2: v.unit2,
      type: this.selectedType
    };

    const handleError = (err: any) => {
      this.loading = false;
      this.errorMsg = err.error?.message || err.error || 'Operation failed.';
    };

    if (this.selectedOp === 'compare') {
      this.quantityService.compare(payload).subscribe({
        next: (res) => {
          this.loading = false;
          this.result = res.result;
          this.resultUnit = '';
        },
        error: handleError
      });
    } else if (this.selectedOp === 'add') {
      this.quantityService.add(payload).subscribe({
        next: (res) => {
          this.loading = false;
          this.result = res.result.toString();
          this.resultUnit = res.unit;
        },
        error: handleError
      });
    } else {
      this.quantityService.subtract(payload).subscribe({
        next: (res) => {
          this.loading = false;
          this.result = res.result.toString();
          this.resultUnit = res.unit;
        },
        error: handleError
      });
    }
  }

  logout(): void {
    this.authService.logout();
  }

  get activeForm(): FormGroup {
    return this.selectedOp === 'convert' ? this.convertForm : this.dualForm;
  }
}