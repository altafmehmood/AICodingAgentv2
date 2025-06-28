import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil, finalize } from 'rxjs';
import { Breach } from '../../models/breach.model';
import { BreachService } from '../../services/breach.service';

@Component({
  selector: 'app-breach-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './breach-list.component.html',
  styleUrls: ['./breach-list.component.css'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class BreachListComponent implements OnInit, OnDestroy {
  breaches: Breach[] = [];
  displayedColumns: string[] = ['title', 'domain', 'breachDate', 'pwnCount', 'dataClasses', 'isVerified', 'actions'];
  loading = false;
  filterForm: FormGroup;
  private destroy$ = new Subject<void>();

  constructor(
    private breachService: BreachService,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {
    this.filterForm = this.formBuilder.group({
      fromDate: [null],
      toDate: [null]
    });
  }

  ngOnInit(): void {
    console.log('BreachListComponent initialized');
    this.loadBreaches();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load breaches with current filter values
   */
  loadBreaches(): void {
    console.log('Loading breaches...');
    this.loading = true;
    this.cdr.detectChanges();
    
    const { fromDate, toDate } = this.filterForm.value;
    console.log('Filter values:', { fromDate, toDate });

    this.breachService.getBreaches(fromDate, toDate)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
          console.log('Loading completed');
        })
      )
      .subscribe({
        next: (breaches) => {
          console.log('Breaches received:', breaches?.length || 0);
          this.breaches = breaches || [];
          this.cdr.detectChanges();
          console.log('Breaches loaded successfully');
        },
        error: (error) => {
          console.error('Error loading breaches:', error);
          this.breaches = [];
          this.cdr.detectChanges();
          this.snackBar.open('Error loading breaches. Please try again.', 'Close', {
            duration: 5000
          });
        }
      });
  }

  /**
   * Apply filters and reload breaches
   */
  applyFilters(): void {
    console.log('Applying filters...');
    this.loadBreaches();
  }

  /**
   * Clear filters and reload breaches
   */
  clearFilters(): void {
    console.log('Clearing filters...');
    this.filterForm.reset();
    this.loadBreaches();
  }

  /**
   * Download PDF report with current filters
   */
  downloadPdf(): void {
    console.log('Downloading PDF...');
    this.loading = true;
    this.cdr.detectChanges();
    
    const { fromDate, toDate } = this.filterForm.value;

    this.breachService.downloadBreachesPdf(fromDate, toDate)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `breach-report-${new Date().toISOString().split('T')[0]}.pdf`;
          link.click();
          window.URL.revokeObjectURL(url);
          this.snackBar.open('PDF downloaded successfully!', 'Close', {
            duration: 3000
          });
        },
        error: (error) => {
          console.error('Error downloading PDF:', error);
          this.snackBar.open('Error downloading PDF. Please try again.', 'Close', {
            duration: 5000
          });
        }
      });
  }

  /**
   * Format date for display
   */
  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  }

  /**
   * Format number with commas
   */
  formatNumber(num: number): string {
    return num.toLocaleString();
  }

  /**
   * Get status color for verification
   */
  getVerificationColor(isVerified: boolean): string {
    return isVerified ? 'accent' : 'warn';
  }
} 