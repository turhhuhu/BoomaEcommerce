import React, { Component } from "react";
import { connect } from "react-redux";
class CartView extends Component {
  state = {};

  handlePurchase = () => {
    console.log(this.props.cart);
  };

  render() {
    return (
      <div className="row col-7 mx-auto">
        <div className="card align-self-start">
          <div className="table-responsive">
            <table className="table table-borderless table-shopping-cart">
              <thead className="text-muted">
                <tr className="small text-uppercase">
                  <th scope="col">Product</th>
                  <th scope="col" width="120">
                    Quantity
                  </th>
                  <th scope="col" width="120">
                    Price
                  </th>
                  <th
                    scope="col"
                    className="text-right d-none d-md-block"
                    width="200"
                  >
                    {" "}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    <figure className="itemside align-items-center">
                      <figcaption className="info">
                        <p href="" className="title text-dark">
                          Camera Canon EOS M50 Kit
                        </p>
                        <p className="text-muted small">
                          Matrix: 25 Mpx <br /> Brand: Canon
                        </p>
                      </figcaption>
                    </figure>
                  </td>
                  <td>
                    <input
                      className="form-control w-75"
                      // onKeyDown={this.handleInputDisable}
                      // min={this.state.MinPriceFilter}
                      // max={this.state.maxPrice}
                      // placeholder={this.state.minPrice}
                      // value={this.state.MaxPriceFilter}
                      // onChange={this.handleChange}
                      name="MaxPriceFilter"
                      type="number"
                    ></input>
                  </td>
                  <td>
                    <div className="price-wrap">
                      <var className="price">$1156.00</var>
                      <small className="text-muted"> $315.20 each </small>
                    </div>
                  </td>
                  <td className="text-right d-none d-md-block">
                    <button className="btn btn-outline-primary"> Remove</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div className="ml-2 col-3">
          <div className="card ">
            <div className="card-body">
              <dl className="dlist-align">
                <dt>Total price:</dt>
                <dd className="text-right">$69.97</dd>
              </dl>
              <dl className="dlist-align">
                <dt>Discount:</dt>
                <dd className="text-right text-danger">- $10.00</dd>
              </dl>
              <dl className="dlist-align">
                <dt>Total:</dt>
                <dd className="text-right text-dark b">
                  <strong>$59.97</strong>
                </dd>
              </dl>
              <hr />

              <button
                onClick={this.handlePurchase}
                className="btn btn-primary btn-block"
              >
                {" "}
                Make Purchase{" "}
              </button>
              <button className="btn btn-light btn-block">
                Continue Shopping
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    cart: store.user.cart,
  };
};

export default connect(mapStateToProps)(CartView);
