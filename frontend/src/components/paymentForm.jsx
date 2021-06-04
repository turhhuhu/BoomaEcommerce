import React, { Component } from "react";
import { connect } from "react-redux";

const monthsArray = [
  "MM",
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

const yearsArray = [
  "YY",
  "2021",
  "2022",
  "2023",
  "2024",
  "2025",
  "2026",
  "2027",
  "2028",
  "2029",
  "2030",
];
class PaymentForm extends Component {
  state = {
    fullName: "",
    id: "",
    cardNumber: "",
    month: "",
    year: "",
    cvv: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handlePayment = () => {
    console.log(this.state);
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
                name="fullName"
                value={this.state.fullName}
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
                      className="form-control"
                      name="month"
                      style={{ width: "120px" }}
                    >
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
                      className="form-control"
                      style={{ width: "100px" }}
                    >
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
                  <label>CVV</label>
                  <input
                    className="form-control"
                    name="cvv"
                    value={this.state.cvv}
                    onChange={this.handleChange}
                    required
                    type="text"
                  ></input>
                </div>
              </div>
            </div>
            <button
              onClick={this.handlePayment}
              className="btn btn-outline-primary btn-block"
              type="button"
            >
              {" "}
              Confirm{" "}
            </button>
          </form>
        </div>
      </div>
    );
  }
}

export default connect()(PaymentForm);
