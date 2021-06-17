import React, { Component } from "react";
import { connect } from "react-redux";
import { removeCartItem } from "../actions/userActions";

class CartItem extends Component {
  state = {
    quantity: 0,
    maxQuantity: 0,
    currentPrice: 0,
  };

  componentDidMount() {
    this.setState({
      quantity: this.props.quantity,
      maxQuantity: this.props.maxQuantity,
      currentPrice: this.props.price * this.props.quantity,
    });
  }

  componentDidUpdate() {
    if (this.props.maxQuantity > this.state.maxQuantity) {
      this.setState({ maxQuantity: this.props.maxQuantity });
    }
  }

  handleInputDisable = (event) => {
    event.preventDefault();
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
          <div className="col">
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
          <div>
            <span className="medium text-dark mr-3">
              <strong>Quantity:</strong> {this.state.quantity}
            </span>
          </div>

          <div className="col-2 price-wrap">
            <span>
              <strong>Price:</strong>
              <var className="price"> ${this.state.currentPrice}</var>
            </span>
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
