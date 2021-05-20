import React, { Component } from "react";
import { connect } from "react-redux";
import { removeCartItem } from "../actions/userActions";

class CartItem extends Component {
  state = {
    quantity: this.props.maxQuantity,
    currentPrice: this.props.price * this.props.maxQuantity,
  };

  handleInputDisable = (event) => {
    event.preventDefault();
  };

  handleQuantityChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
    this.setState({ currentPrice: this.props.price * event.target.value });
  };

  removeCartItem = () => {
    this.props.dispatch(
      removeCartItem(this.props.basketGuid, this.props.purchaseProductGuid)
    );
  };

  render() {
    return (
      <article className="card card-body mb-3">
        <div className="row align-items-center">
          <div className="col-md-6">
            <figcaption className="info">
              <h6 className="title text-dark">{this.props.product?.name}</h6>
              <span className="text-muted small">
                Store: {this.props.storeName}
              </span>
              <br />
              <span className="text-muted small">
                Category: {this.props.product?.category}
              </span>
            </figcaption>
          </div>
          <div className="col-md-3">
            <form className="form-inline">
              <div className="form-group">
                <div className="mr-3">
                  <label htmlFor="quantity" className="medium text-dark">
                    quantity:{" "}
                  </label>
                  <label htmlFor="quantity" className="small text-muted">
                    max quantity: {this.props.maxQuantity}
                  </label>
                </div>
                <input
                  id="quantity"
                  className="form-control w-60"
                  onKeyDown={this.handleInputDisable}
                  min={1}
                  max={this.props.maxQuantity}
                  placeholder={this.state.minPrice}
                  value={this.state.quantity}
                  onChange={this.handleQuantityChange}
                  name="quantity"
                  type="number"
                ></input>
              </div>
            </form>
          </div>

          <div className="col price-wrap">
            <var className="price">{this.state.currentPrice}$</var>
            <br />
            <small className="text-muted"> {this.props.price}$ each </small>
          </div>
          <div className="pr-2">
            <button
              onClick={this.removeCartItem}
              className="btn btn-outline-primary"
            >
              {" "}
              Remove
              <i className="ml-2 fa fa-trash"></i>
            </button>
          </div>
        </div>
      </article>
    );
  }
}

export default connect()(CartItem);
