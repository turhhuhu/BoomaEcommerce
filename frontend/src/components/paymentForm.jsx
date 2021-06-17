import { Alert } from "@material-ui/lab";
import React, { Component } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { submitPaymentInfo } from "../actions/userActions";
import { yearsArray, monthsArray } from "../utils/constants";
import { isNormalInteger } from "../utils/utilFunctions";

class PaymentForm extends Component {
  state = {
    error: undefined,
    holderName: this.props.paymentInfo.holderName
      ? this.props.paymentInfo.holderName
      : "",
    id: this.props.paymentInfo.id ? this.props.paymentInfo.id : "",
    cardNumber: this.props.paymentInfo.cardNumber
      ? this.props.paymentInfo.cardNumber
      : "",
    month: this.props.paymentInfo.month ? this.props.paymentInfo.month : "",
    year: this.props.paymentInfo.year ? this.props.paymentInfo.year : "",
    ccv: this.props.paymentInfo.ccv ? this.props.paymentInfo.ccv : "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handlePayment = (isWithDelivery) => {
    if (
      !this.state.holderName ||
      !this.state.id ||
      !this.state.cardNumber ||
      !this.state.month ||
      !this.state.year ||
      !this.state.ccv
    ) {
      this.setState({ error: "Please fill out all fields" });
      return;
    }
    if (
      !isNormalInteger(this.state.id) ||
      !isNormalInteger(this.state.cardNumber) ||
      !isNormalInteger(this.state.ccv)
    ) {
      this.setState({ error: "Id, Card number, and Ccv must be numbers" });
      return;
    }
    this.props.dispatch(
      submitPaymentInfo({
        holderName: this.state.holderName,
        id: this.state.id,
        cardNumber: this.state.cardNumber,
        month: this.state.month,
        year: this.state.year,
        ccv: this.state.ccv,
      })
    );
    if (isWithDelivery) {
      this.props.history.push("/cart/delivery");
    } else {
      this.props.history.push("/cart/purchase");
    }
  };

  render() {
    return (
      <div
        className="row col-3 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <div className="card-body">
          <h4 className="card-title mb-4">Payment info</h4>
          <form>
            <div className="form-group">
              <label>Name on card:</label>
              <input
                type="text"
                className="form-control"
                name="holderName"
                value={this.state.holderName}
                onChange={this.handleChange}
                required
              ></input>
            </div>
            <div className="form-group">
              <label>ID:</label>
              <input
                type="text"
                className="form-control"
                name="id"
                value={this.state.id}
                onChange={this.handleChange}
                required
              ></input>
            </div>

            <div className="form-group">
              <label>Card number:</label>
              <div className="input-group">
                <input
                  type="text"
                  className="form-control"
                  name="cardNumber"
                  value={this.state.cardNumber}
                  onChange={this.handleChange}
                ></input>
                <div className="input-group-append">
                  <span className="input-group-text">
                    <i className="fa fa-cc-visa"></i>{" "}
                    <i className="fa fa-cc-amex ml-1"></i>
                    <i className="fa fa-cc-mastercard ml-1"></i>
                  </span>
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col-9">
                <div className="form-group">
                  <label>
                    <span className="hidden-xs">Expiration date:</span>{" "}
                  </label>
                  <div className="form-inline" style={{ minWidth: "220px" }}>
                    <select
                      onChange={this.handleChange}
                      defaultValue={this.state.month ? this.state.month : null}
                      className="form-control"
                      name="month"
                      style={{ width: "120px" }}
                    >
                      <option hidden>MM</option>
                      {monthsArray.map((month, index) => (
                        <option value={index} key={index}>
                          {month}
                        </option>
                      ))}
                    </select>
                    <span style={{ width: "20px", textAlign: "center" }}>
                      {" "}
                      /{" "}
                    </span>
                    <select
                      onChange={this.handleChange}
                      name="year"
                      defaultValue={this.state.year ? this.state.year : null}
                      className="form-control"
                      style={{ width: "100px" }}
                    >
                      <option hidden>YY</option>
                      {yearsArray.map((year, index) => (
                        <option value={year} key={index}>
                          {year}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
              <div className="col-3">
                <div className="form-group">
                  <label>CCV</label>
                  <input
                    className="form-control"
                    name="ccv"
                    value={this.state.ccv}
                    onChange={this.handleChange}
                    required
                    type="text"
                  ></input>
                </div>
              </div>
            </div>
            <button
              onClick={() => this.handlePayment(false)}
              className="btn btn-primary btn-block"
              type="button"
            >
              {" "}
              Continue to review purchase{" "}
            </button>
            <button
              onClick={() => this.handlePayment(true)}
              className="btn btn-outline-info btn-block"
              type="button"
            >
              {" "}
              Continue and order delivery{" "}
            </button>
            {(this.props.error || this.state.error) && (
              <div className="mt-2">
                <Alert severity="error" onClick={() => this.setState(null)}>
                  {this.props.error || this.state.error}
                </Alert>
              </div>
            )}
          </form>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    paymentInfo: store.user.paymentInfo,
  };
};

export default withRouter(connect(mapStateToProps)(PaymentForm));
