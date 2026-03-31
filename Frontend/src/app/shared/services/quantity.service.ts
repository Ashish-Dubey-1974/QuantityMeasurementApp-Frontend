import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';

function toPascal(s: string): string {
  return s.charAt(0).toUpperCase() + s.slice(1);
}

interface BackendQuantityResult { value: number; unitSymbol: string; }
interface BackendCompareResult  { areEqual: boolean; }

export interface ConversionResult { result: number; unit: string; }
export interface QuantityResult   { result: number; unit: string; }
export interface CompareResult    { result: string; areEqual: boolean; }

@Injectable({ providedIn: 'root' })
export class QuantityService {
  private apiUrl = `${environment.apiUrl}/quantitymeasurement`;
  constructor(private http: HttpClient) {}

  convert(req: { value: number; fromUnit: string; toUnit: string; type: string }): Observable<ConversionResult> {
    return this.http.post<BackendQuantityResult>(`${this.apiUrl}/convert`, {
      QuantityType: toPascal(req.type), Value: req.value,
      SourceUnit: req.fromUnit, TargetUnit: req.toUnit
    }).pipe(map(r => ({ result: r.value, unit: r.unitSymbol })));
  }

  add(req: { value1: number; unit1: string; value2: number; unit2: string; type: string }): Observable<QuantityResult> {
    return this.http.post<BackendQuantityResult>(`${this.apiUrl}/add`, {
      QuantityType: toPascal(req.type),
      Value1: req.value1, Unit1: req.unit1, Value2: req.value2, Unit2: req.unit2
    }).pipe(map(r => ({ result: r.value, unit: r.unitSymbol })));
  }

  subtract(req: { value1: number; unit1: string; value2: number; unit2: string; type: string }): Observable<QuantityResult> {
    return this.http.post<BackendQuantityResult>(`${this.apiUrl}/subtract`, {
      QuantityType: toPascal(req.type),
      Value1: req.value1, Unit1: req.unit1, Value2: req.value2, Unit2: req.unit2,
      ResultUnit: req.unit1
    }).pipe(map(r => ({ result: r.value, unit: r.unitSymbol })));
  }

  compare(req: { value1: number; unit1: string; value2: number; unit2: string; type: string }): Observable<CompareResult> {
    return this.http.post<BackendCompareResult>(`${this.apiUrl}/compare`, {
      QuantityType: toPascal(req.type),
      Value1: req.value1, Unit1: req.unit1, Value2: req.value2, Unit2: req.unit2
    }).pipe(map(r => ({ areEqual: r.areEqual, result: r.areEqual ? '✅ Equal' : '❌ Not Equal' })));
  }
}