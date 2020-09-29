using System;
using Xamarin.Forms;

namespace TipCalculator
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Amount of the bill, in dollars
        /// </summary>
        public double billAmount = 0;
        /// <summary>
        /// Amount of the tip, in dollars
        /// </summary>
        public double tipAmount;
        /// <summary>
        /// Total amount (bill plus tip), in dollars 
        /// </summary>
        public double totalAmount;
        /// <summary>
        /// Tip percentage, as a number between 0 and 100
        /// </summary>
        public double tipPercent;
        /// <summary>
        /// Number of diners to split bill between
        /// </summary>
        int numberOfDiners = 1;
        /// <summary>
        /// Whether the user started to enter bill amount
        /// </summary>
        bool hasStarted = false;
        /// <summary>
        /// Whether the user has typed a decimal point
        /// </summary>
        bool hasTypedDecimal = false;
        /// <summary>
        /// The number of decimal digits already entered
        /// </summary>
        int numberOfDecimalDigits = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle slider change events, updating the tip percentage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            tipPercent = Math.Ceiling(e.NewValue);
            percentageLabel.Text = tipPercent + "%";

            // Update the rest of the UI
            UpdateUI();
        }

        /// <summary>
        /// Updates the portion of the UI that is above the number pad.
        /// </summary>
        private void UpdateUI()
        {
            // Calculate the tip value and total
            tipAmount = Math.Floor(billAmount * tipPercent) / 100;
            totalAmount = billAmount + tipAmount;
            // Set the text for tip, total, cost-per-diner label controls
            tipAmountLabel.Text = "$" + String.Format("{0:0.00}", tipAmount);
            totalAmountLabel.Text = "$" + String.Format("{0:0.00}", totalAmount);
            costPerDinerLabel.Text = "$" + String.Format("{0:0.00}", totalAmount/numberOfDiners);

            /* The bill amount is displayed with two labels: the amount enetered by the user, plus
             * a placeholder for parts not yet entered, but required for correect currency formatting
             */
            if (!hasStarted)
            {
                billAmountLabel.Text = "$";
                billPlaceholderLabel.Text = "0.00";
            }
            else if (!hasTypedDecimal)
            {
                billAmountLabel.Text = String.Format("${0:0}", billAmount);
                billPlaceholderLabel.Text = ".00";
            }
            else if (numberOfDecimalDigits == 0)
            {
                billAmountLabel.Text = String.Format("${0:0}.", billAmount);
                billPlaceholderLabel.Text = "00";
            }
            else if (numberOfDecimalDigits == 1)
            {
                billAmountLabel.Text = String.Format("${0:0.0}", billAmount);
                billPlaceholderLabel.Text = "0";
            }
            else
            {
                billAmountLabel.Text = String.Format("${0:0.00}", billAmount);
                billPlaceholderLabel.Text = "";
            }
        }

        /// <summary>
        /// Handles click events for the number-pad buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numpad_Button_Clicked(object sender, EventArgs e)
        {
            // Get label from event sender
            string buttonLabel = ((Button)sender).Text;
            // User has started entering the amount (unless they initally pushed a zero)
            if (!hasStarted && buttonLabel != "0") {
                hasStarted = true;
            }
            switch(buttonLabel) {
                case "C":
                    {
                        // Reset everything
                        billAmount = 0;
                        hasStarted = false;
                        hasTypedDecimal = false;
                        numberOfDecimalDigits = 0;
                        break;
                    }
                case ".":
                    {
                        hasTypedDecimal = true;
                        break;
                    }
                case "0":
                    {
                        // Behaviour depends on whether user has started to enter amount 
                        if(hasStarted && !hasTypedDecimal)
                        {
                            billAmount *= 10;
                        } else if(hasStarted && hasTypedDecimal && numberOfDecimalDigits < 2)
                        {
                            numberOfDecimalDigits++;
                        }
                        break;
                    }
                default:
                    {
                        // Get the digit as a number
                        double digit = double.Parse(buttonLabel);
                        if (!hasTypedDecimal)
                        {
                            billAmount = billAmount * 10 + digit;
                        } else if(numberOfDecimalDigits < 2)
                        {
                            numberOfDecimalDigits++;
                            billAmount += digit * Math.Pow(0.1, numberOfDecimalDigits);
                        }
                        break;
                    }

            }
            // Now update the UI to reflect the new amounts
            UpdateUI();
        }

        /// <summary>
        /// Handles changes to the numebr of diners stepper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Get the new value as an interger
            numberOfDiners = (int)e.NewValue;
            // Update the amount in the number of diners label
            numberOfDinersLabel.Text = numberOfDiners.ToString();
            // Update the rest of the UI
            UpdateUI();
        }
    }
}
